using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ParkPal.API.Services.Interfaces;
using ParkPal.Common.API.Models.Dtos;
using ParkPal.Common.Data.Interfaces;

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
            if (string.IsNullOrEmpty(registration.DeviceToken)) 
                return BadRequest("Device token is required.");

            // ⭐️ Clean separation of concerns!
            await usersRepository.RegisterDeviceHandshakeAsync(registration);
            var signedToken = tokenService.GenerateToken(registration.AppUserId);

            // 3. Return it to iOS
            return Ok(new { token = signedToken });
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"❌ Handshake failed: {ex.Message}");
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