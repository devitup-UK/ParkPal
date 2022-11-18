using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkPal.API.Models;
using ParkPal.API.Models.Enums;
using ParkPal.API.Models.Requests.Notification;
using ParkPal.API.Models.Requests.Subscription;
using ParkPal.API.Models.Requests.ThemePark;
using ParkPal.API.Models.Requests.Token;
using ParkPal.API.Models.Responses.Notification;
using ParkPal.API.Services.Interfaces;
using ParkPal.Common.Models;
using ParkPal.Common.Models.Database.Entities.Device;
using ParkPal.Common.Models.Database.Entities.Notification;
using ParkPal.Common.Services.Interfaces;

namespace ParkPal.API.Controllers;

[AllowAnonymous]
[ApiController]
[Route("themepark")]
public class ThemeParkController : ControllerBase
{
    private readonly IThemeParkService _themeParkService;
    private readonly ILogger<ThemeParkController> _logger;

    public ThemeParkController(ILogger<ThemeParkController> logger, IThemeParkService themeParkService)
    {
        _logger = logger;
        _themeParkService = themeParkService;
    }
    
    [HttpGet("destinations")]
    public IActionResult Destinations()
    {
        // Call the theme park API.
        return Ok(_themeParkService.GetDestinations());
    }

    [HttpGet("destinations/{destinationId}/parks")]
    public IActionResult Parks(string destinationId)
    {
        Destination? destination = _themeParkService.GetDestinationWithParks(destinationId);

        if (destination != null)
        {
            return Ok(destination);
        }

        return StatusCode(500);
    }

    [HttpPost("parks/{parkId}/attractions")]
    public IActionResult Attractions(string parkId, [FromBody] AttractionsRequest request)
    {
        Park? park = _themeParkService.GetParkWithAttractions(parkId);

        if (park != null)
        {

            // We have all of our attractions, we just need to sort them.
            switch (request.Filters.Type)
            {
                case WaitTimeFilterType.Favourites:
                    // Sort by Ids in favouriteIds.
                    park.Attractions = park.Attractions
                        .Where(a => request.FavouriteAttractionIds.Contains(a.AttractionId)).ToList();
                    break;
            }

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
            }

            return Ok(park);
        }

        return StatusCode(500);
    }
}