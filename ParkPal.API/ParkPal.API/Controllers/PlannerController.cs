using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkPal.API.Models;
using ParkPal.API.Services.Interfaces;
using ParkPal.Common.API.Models.Dtos;
using ParkPal.Common.Data.Interfaces;

namespace ParkPal.API.Controllers;

[ApiController]
[Route("planner")]
[Authorize]
public class PlannerController(IPlanningService planningService, IItineraryRepository itineraryRepository) : ControllerBase
{
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateItinerary([FromBody] GeneratePlanRequestDto request)
    {
        try
        {
            var userId = User.Identity?.Name;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Inject the ID securely so nobody can create plans for other users
            request.AppUserId = userId;

            var plan = await planningService.GenerateItineraryAsync(request);
            
            return Ok(plan);
        }
        catch (Exception ex)
        {
            // Log the error in your real app!
            return StatusCode(500, new { message = "Failed to generate itinerary.", details = ex.Message });
        }
    }
    
    [HttpPost("save")]
    public async Task<IActionResult> SaveItinerary([FromBody] SavedPlanDto plan)
    {
        try
        {
            // Extract the user's ID from the JWT token
            var userId = User.Identity?.Name;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Hand the plan straight to the repository we built earlier!
            await planningService.SavePlanAsync(userId, plan);
            
            return Ok(new { message = "Plan saved to ParkPal successfully!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error saving itinerary.");
        }
    }
    
    [HttpGet("my-plans")]
    public async Task<IActionResult> GetMyPlans()
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var plans = await itineraryRepository.GetUserPlansAsync(userId);
        return Ok(plans);
    }
    
    [HttpGet("plan/{planId}")]
    public async Task<IActionResult> GetPlan(string planId)
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var plans = await itineraryRepository.GetPlanByIdAsync(planId, userId);
        return Ok(plans);
    }
    
    [HttpGet("catalog/widget-plans")]
    public async Task<IActionResult> GetLightweightPlans()
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var plans = await itineraryRepository.GetUserPlansAsync(userId);
        
        return Ok(plans.Select(a => new LightweightSavedPlanDto()
        {
            Id = a.Id,
            Title = a.Title,
            DestinationName = a.DestinationName,
            ParkName = a.ParkName,
            TripDate = a.TripDate,
            IsOwner = a.IsOwner,
        }));
    }
    
    [HttpDelete("delete/{planId}")]
    public async Task<IActionResult> DeleteItinerary(Guid planId)
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        await itineraryRepository.DeletePlanAsync(userId, planId);
        return Ok(new { message = "Plan deleted." });
    }
    
    [HttpPatch("rename/{planId}")]
    public async Task<IActionResult> RenameItinerary(Guid planId, [FromBody] RenamePlanRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.NewTitle)) return BadRequest("Title cannot be empty.");

        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        await itineraryRepository.RenamePlanAsync(userId, planId, request.NewTitle);
    
        return Ok(new { message = "Plan renamed successfully." });
    }
    
    [HttpGet("shared/{shareCode}")]
    public async Task<IActionResult> GetSharedPlanPreview(string shareCode)
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var plan = await itineraryRepository.GetPlanPreviewByShareCodeAsync(shareCode);
        
        if (plan == null) return NotFound();
        
        return Ok(plan);
    }
    
    [HttpPost("shared/{shareCode}/join")]
    public async Task<IActionResult> JoinSharedPlan([FromRoute] string shareCode)
    {
        // Grab the user ID from your auth claims (adjust this to match your auth setup!)
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var success = await itineraryRepository.JoinPlanByShareCodeAsync(userId, shareCode);
    
        if (!success) 
        {
            return NotFound("Invalid share code or plan no longer exists.");
        }

        return Ok(); 
    }
    
    [HttpDelete("shared/{planId}/leave")]
    public async Task<IActionResult> LeaveSharedPlan([FromRoute] Guid planId)
    {
        // Grab the user ID from your auth claims (adjust this to match your auth setup!)
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var success = await itineraryRepository.LeavePlanAsync(userId, planId);
    
        if (!success) 
        {
            return NotFound("Invalid share code or plan no longer exists.");
        }

        return Ok(); 
    }
}