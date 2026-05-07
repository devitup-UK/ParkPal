using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkPal.Common.API.Models.Dtos;
using ParkPal.Common.Data.Interfaces;

namespace ParkPal.API.Controllers;

[Authorize] // 🔒 Assumes they are logged in!
[ApiController]
[Route("live-activities")]
public class LiveActivityController(ILiveActivityRepository repo) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterToken([FromBody] RegisterLiveActivityRequest request)
    {
        // Grab the user ID from your auth token
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            await repo.RegisterMonitorAsync(userId, request);
            return Ok(new { message = "Live Activity token registered securely!" });
        }
        catch (Exception ex)
        {
            // Log the exception here
            return StatusCode(500, "Failed to register Live Activity token.");
        }
    }
    
    [HttpDelete("remove/{attractionId}")]
    public async Task<IActionResult> RemoveToken(string attractionId)
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        await repo.RemoveMonitorAsync(userId, attractionId);
        return Ok();
    }
}