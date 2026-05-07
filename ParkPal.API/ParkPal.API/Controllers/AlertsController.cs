using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkPal.Common.API.Models;
using ParkPal.Common.API.Models.Dtos;
using ParkPal.Common.Data.Interfaces;
using ParkPal.Common.Models;

namespace ParkPal.API.Controllers;

[ApiController]
[Route("alerts")]
[Authorize] // ⭐️ 1. Lock down the entire controller!
public class AlertsController(IAlertRepository _alertRepository) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAlert([FromBody] CreateAlertRequest request)
    {
        // ⭐️ 2. Grab the trusted ID from the JWT token
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        if (request.TargetWaitTime < 0)
        {
            return BadRequest(new { Message = "Target wait time cannot be negative." });
        }

        if (string.IsNullOrWhiteSpace(request.AttractionId))
        {
            return BadRequest(new { Message = "Attraction ID is required." });
        }

        // ⭐️ 3. Override whatever the app sent with our trusted token ID
        request.AppUserId = userId; 

        var success = await _alertRepository.UpsertAlertAsync(request);

        if (!success)
        {
            return StatusCode(500, new { Message = "Failed to save the alert." });
        }

        return Created(string.Empty, new { Message = "Alert created successfully!" });
    }
    
    // ⭐️ 4. We don't need {appUserId} in the URL anymore!
    [HttpGet] 
    public async Task<ActionResult<List<UserAlertDto>>> GetUserAlerts()
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var alerts = await _alertRepository.GetUserAlertsAsync(userId);
        return Ok(alerts);
    }
    
    [HttpDelete("{attractionId}")]
    public async Task<IActionResult> DeleteAlert(string attractionId)
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var success = await _alertRepository.DeleteAlertAsync(userId, attractionId);
        
        if (!success) return NotFound(new { Message = "Alert not found." });
        
        return Ok(new { Message = "Alert deleted." });
    }

    [HttpPatch("{attractionId}/status")]
    public async Task<IActionResult> ToggleAlertStatus(string attractionId, [FromBody] ToggleAlertDto request)
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var success = await _alertRepository.ToggleAlertStatusAsync(userId, attractionId, request.IsActive);
        
        if (!success) return NotFound(new { Message = "Alert not found." });
        
        return Ok(new { Message = "Status updated." });
    }
}