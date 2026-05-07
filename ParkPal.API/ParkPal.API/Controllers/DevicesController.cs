using Microsoft.AspNetCore.Mvc;
using ParkPal.API.Models;
using ParkPal.Common.Data.Interfaces;

namespace ParkPal.API.Controllers;

[ApiController]
[Route("devices")]
public class DevicesController(IDeviceRepository deviceRepository) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.AppUserId) || string.IsNullOrWhiteSpace(request.DeviceToken))
        {
            return BadRequest(new { Message = "AppUserId and DeviceToken are required." });
        }

        await deviceRepository.UpsertDeviceAsync(request.AppUserId, request.DeviceToken);
        
        return Ok();
    }
}