using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkPal.API.Models.Enums;
using ParkPal.API.Models.Requests.ThemePark;
using ParkPal.Common.API.Models;
using ParkPal.Common.API.Models.Dtos;
using ParkPal.Common.Data.Interfaces;
using ParkPal.Common.Helpers;
using ParkPal.Common.Models.Enums;

namespace ParkPal.API.Controllers;

[ApiController]
[Route("themepark")]
// ⭐️ IThemeParkService is completely removed! Just the Logger and Repository now.
public class ThemeParkController(ILogger<ThemeParkController> _logger, IParkRepository parkRepository, ICrowdSourceRepository crowdSourceRepository, IUsersRepository usersRepository) : ControllerBase
{
    [HttpGet("destinations")]
    [AllowAnonymous]
    public async Task<IActionResult> Destinations()
    {
        // ⭐️ Pulls straight from the Postgres DB now!
        var destinations = await parkRepository.GetActiveDestinationsAsync();
        return Ok(destinations);
    }

    [HttpGet("destinations/{destinationId}/parks")]
    [AllowAnonymous]
    public async Task<IActionResult> Parks(string destinationId)
    {
        // ⭐️ Uses the repo to grab the parks AND their images in one query
        var destination = await parkRepository.GetDestinationWithParksAsync(destinationId);

        if (destination != null)
        {
            return Ok(destination);
        }

        return StatusCode(500, "Unable to fetch destination parks.");
    }
    
    // ⭐️ The new lightweight GET endpoint!
    [HttpGet("parks/{parkId}/attractions")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAttractions(string parkId)
    {
        var attractions = await parkRepository.GetParkAttractionsAsync(parkId);
        
        if (!attractions.Any()) return NotFound("No attractions found for this park.");
        
        return Ok(attractions);
    }

    [HttpPost("parks/{parkId}/attractions")]
    [AllowAnonymous]
    public async Task<IActionResult> Attractions(string parkId, [FromBody] AttractionsRequest request)
    {
        // ⭐️ Uses the awesome LEFT JOIN query we wrote earlier
        var park = await parkRepository.GetParkWithLiveAttractionsAsync(parkId);

        if (park != null)
        {
            // Apply filtering logic
            if (request.Filters.Type == WaitTimeFilterType.Favourites)
            {
                park.Attractions = park.Attractions
                    .Where(a => request.FavouriteIds.Contains(a.AttractionId)).ToList();
            }

            // Apply sorting logic
            // Apply sorting logic
            switch (request.Filters.Sort)
            {
                case WaitTimeFilterSort.ThrillRides:
                    park.Attractions = park.Attractions.OrderByDescending(a => a.Thrill).ToList();
                    break;
                case WaitTimeFilterSort.TameRides:
                    park.Attractions = park.Attractions.OrderByDescending(a => !a.Thrill).ToList();
                    break;
                case WaitTimeFilterSort.HighestWaitTime:
                    park.Attractions = park.Attractions.OrderByDescending(a => a.WaitTime).ToList();
                    break;
                case WaitTimeFilterSort.LowestWaitTime:
                    park.Attractions = park.Attractions.OrderBy(a => a.WaitTime).ToList();
                    break;
                
                // ⭐️ NEW: Alphabetical (Perfect for Dining)
                case WaitTimeFilterSort.Alphabetical:
                    park.Attractions = park.Attractions.OrderBy(a => a.Name).ToList();
                    break;
                
                // ⭐️ NEW: Starting Soon (Perfect for Shows)
                case WaitTimeFilterSort.StartingSoon:
                    var now = DateTimeOffset.UtcNow;
                    park.Attractions = park.Attractions
                        .OrderBy(a => a.Showtimes != null && a.Showtimes.Any(s => s.StartTime > now) 
                            // If it has future shows, sort by the closest one!
                            ? a.Showtimes.Where(s => s.StartTime > now).Min(s => s.StartTime) 
                            // If no shows left, push it to the absolute bottom!
                            : DateTimeOffset.MaxValue) 
                        .ToList();
                    break;
            }

            return Ok(park);
        }

        return StatusCode(500, "Unable to fetch park attractions.");
    }
    
    [HttpGet("attraction/{id}/chart")]
    [AllowAnonymous]
    public async Task<ActionResult<AttractionChartResponse>> GetChartData(string id)
    {
        var chartData = await parkRepository.GetAttractionChartDataAsync(id);

        if (chartData.HistoricalData.Count == 0)
        {
            return NotFound();
        }

        return Ok(chartData);
    }
    
    [HttpPost("attraction/submit")]
    [Authorize]
    public async Task<IActionResult> SubmitAttractionState([FromBody] AttractionSubmissionDto submission)
    {
        var userId = User.Identity?.Name;

        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            // Basic validation
            if (string.IsNullOrEmpty(submission.AttractionId))
            {
                return BadRequest("AttractionId is required.");
            }
            
            // 1. Get the Park's location from your DB based on the AttractionId they submitted
            var parkLocation = await parkRepository.GetParkLocationForAttractionAsync(submission.AttractionId);

            if (parkLocation.Latitude.HasValue && parkLocation.Longitude.HasValue)
            {
                // 2. Ask the Bouncer how far away the user is
                double distanceMiles = GeoHelper.CalculateDistanceInMiles(
                    submission.Latitude, 
                    submission.Longitude, 
                    parkLocation.Latitude.Value, 
                    parkLocation.Longitude.Value
                );

                // 3. The 2-Mile Rule (Accounts for massive parks and slight GPS drift)
                if (distanceMiles > 2.0)
                {
                    _logger.LogWarning($"🛑 GEOFENCE BREACH: User {userId} tried to submit a time from {distanceMiles:F1} miles away!");
        
                    // Silently tank their Trust Score! 
                    await usersRepository.DecreaseUserTrustScoreAsync(userId, 50);
        
                    // ⭐️ CRITICAL: Still return an OK so the troll thinks it worked
                    return Ok(new { success = true }); 
                }
            }

            // If they passed the Geofence, proceed with saving the valid wait time...

            // 🎢 Log the submission to the CrowdSource schema
            await crowdSourceRepository.SubmitAttractionStateAsync(userId, submission);
            await usersRepository.IncreaseUserTrustScoreAsync(userId);

            // 🍻 Return a 200 OK so the app knows it worked
            return Ok(new { message = "Report submitted successfully! Thanks for helping out, mate." });
        }
        catch (Exception ex)
        {
            // Log the error (using your preferred logger)
            Console.WriteLine($"❌ Error saving crowd submission: {ex.Message}");
            return StatusCode(500, "Internal server error saving submission.");
        }
    }
    
    // ⭐️ The Hierarchical Catalog for the iOS Widget!
    [HttpGet("catalog/widget-rides")]
    [AllowAnonymous]
    public async Task<IActionResult> GetWidgetRideCatalog()
    {
        var catalog = await parkRepository.GetWidgetEntityCatalogByTypeAsync(EntityTypeStrings.Attraction);
        
        if (catalog.Destinations.Count == 0) 
        {
            return NotFound("No catalog ride data found.");
        }
        
        return Ok(catalog);
    }
    
    // ⭐️ The Hierarchical Catalog for the iOS Widget!
    [HttpGet("catalog/widget-shows")]
    [AllowAnonymous]
    public async Task<IActionResult> GetWidgetShowCatalog()
    {
        var catalog = await parkRepository.GetWidgetEntityCatalogByTypeAsync(EntityTypeStrings.Show);
        
        if (catalog.Destinations.Count == 0) 
        {
            return NotFound("No catalog show data found.");
        }
        
        return Ok(catalog);
    }
    
    // ⭐️ The Hierarchical Catalog for the iOS Widget!
    [HttpGet("catalog/widget-restaurants")]
    [AllowAnonymous]
    public async Task<IActionResult> GetWidgetRestaurantCatalog()
    {
        var catalog = await parkRepository.GetWidgetEntityCatalogByTypeAsync(EntityTypeStrings.Restaurant);
        
        if (catalog.Destinations.Count == 0) 
        {
            return NotFound("No catalog restaurant data found.");
        }
        
        return Ok(catalog);
    }
    
    // ⭐️ The dedicated microscopic endpoint for the iOS Ride Widget engine!
    [HttpGet("attraction/{id}/live")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLiveAttractionStatus(string id)
    {
        var status = await parkRepository.GetLiveAttractionStatusAsync(id);
        
        if (status == null) 
        {
            return NotFound($"No attraction found with ID: {id}");
        }
        
        return Ok(status);
    }
    
    // ⭐️ The dedicated microscopic endpoint for the iOS Show Widget engine!
    [HttpGet("show/{id}/live")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLiveShowtimes(string id)
    {
        var status = await parkRepository.GetUpcomingShowtimesAsync(id);
        
        if (status == null) 
        {
            return NotFound($"No show found with ID: {id}");
        }
        
        return Ok(status);
    }
    
    // ⭐️ Return restaurants for a given parkId.
    [HttpGet("parks/{parkId}/restaurants")]
    [AllowAnonymous]
    public async Task<IActionResult> GetParkRestaurants(string parkId)
    {
        var restaurants = await parkRepository.GetRestaurantsForParkAsync(parkId);
    
        // ⭐️ THE FIX: Added the '!' so it 404s when the list is EMPTY.
        if (!restaurants.Any()) 
        {
            return NotFound($"No restaurants found for park with ID: {parkId}");
        }
    
        return Ok(restaurants);
    }
    
    // ⭐️ Return shows AND their projected times for a specific date!
    // Example: GET /themepark/parks/dae968d5-630d-4719-8b06-3d107e944401/shows?tripDate=2026-10-14
    [HttpGet("parks/{parkId}/shows")]
    [AllowAnonymous]
    public async Task<IActionResult> GetParkShows(string parkId, [FromQuery] DateTime tripDate)
    {
        var shows = await parkRepository.GetShowsForParkAsync(parkId, tripDate);
    
        if (!shows.Any()) 
        {
            return NotFound($"No shows found for park with ID: {parkId}");
        }
    
        return Ok(shows);
    }
}