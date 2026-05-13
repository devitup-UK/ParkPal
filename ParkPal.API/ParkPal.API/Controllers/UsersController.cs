using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ParkPal.API.Services.Interfaces;
using ParkPal.Common.API.Models.Dtos;
using ParkPal.Common.Data.Interfaces;
using ParkPal.Common.Models;

namespace ParkPal.API.Controllers;

[Authorize]
[ApiController]
[Route("users")]
// ⭐️ IThemeParkService is completely removed! Just the Logger and Repository now.
public class UsersController(ILogger<UsersController> _logger, IUsersRepository usersRepository, ITokenService tokenService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDto registration)
    {
        try 
        {
            // 1. Ensure we at least have an AppUserId (from RevenueCat or generated UUID)
            if (string.IsNullOrEmpty(registration.AppUserId)) 
                return BadRequest("AppUserId is required.");

            // 2. ALWAYS create the Profile, regardless of push notifications!
            await usersRepository.RegisterProfileAsync(registration.AppUserId);

            // 3. ONLY register the device token if they actually gave us one
            if (!string.IsNullOrEmpty(registration.DeviceToken))
            {
                await usersRepository.RegisterDeviceTokenAsync(registration);
            }

            // 4. ALWAYS give them a JWT so they can use the app!
            var signedToken = tokenService.GenerateToken(registration.AppUserId);
            return Ok(new { token = signedToken });
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"❌ Handshake failed: {ex.Message}");
            return StatusCode(500);
        }
    }
    
    [HttpPost("device-token")]
    public async Task<IActionResult> UpdateDeviceToken([FromBody] UpdateTokenDto request)
    {
        try
        {
            // Extract the AppUserId securely from the JWT claim we set earlier
            var userId = User.Identity?.Name; 
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            if (string.IsNullOrEmpty(request.DeviceToken))
                return BadRequest("Token is required.");

            // We can reuse the exact same repository method from the registration flow!
            var registration = new UserRegistrationDto 
            { 
                AppUserId = userId, 
                DeviceToken = request.DeviceToken 
            };
        
            await usersRepository.RegisterDeviceTokenAsync(registration);
        
            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Token update failed: {ex.Message}");
            return StatusCode(500);
        }
    }
    
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = User.Identity?.Name;
        if (userId == null) return Unauthorized();

        var profile = await usersRepository.GetProfileAsync(userId);
        if (profile == null) return NotFound();

        return Ok(new {
            totalSubmissions = profile.TotalSubmissions,
            trustScore = profile.TrustScore,
            joinedAt = profile.FirstSeenAt
        });
    }
}