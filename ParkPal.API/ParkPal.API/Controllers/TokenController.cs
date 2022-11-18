using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ParkPal.API.Models;
using ParkPal.API.Models.Requests.Token;
using ParkPal.API.Models.Responses.Token;
using ParkPal.API.Services.Interfaces;
using ParkPal.Common.Models.Configuration;
using ParkPal.Common.Models.Database.Entities.Device;

namespace ParkPal.API.Controllers;

[AllowAnonymous]
[ApiController]
[Route("token")]
public class TokenController : ControllerBase
{
    private ITokenService _tokenService;
    private readonly ILogger<TokenController> _logger;

    public TokenController(ILogger<TokenController> logger, ITokenService tokenService)
    {
        _logger = logger;
        _tokenService = tokenService;
    }
    
    // Step 1 - The app calls this endpoint if they have a token and we verify that it is legitimate.
    [HttpPost("verify")]
    public IActionResult Verify([FromBody]VerifyTokenRequest request)
    {
        if(_tokenService.Verify(request.Token))
        {
            return Ok();
        }

        return Unauthorized();
    }
    
    // Alternative to Step 1 - The app calls this endpoint if they have a token and we can't verify, so they request a new one.
    [HttpPost("generate")]
    public IActionResult Generate()
    {
        Token generatedToken = _tokenService.Generate();
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Settings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, generatedToken.Value)
            }),
            Expires = DateTime.UtcNow.AddYears(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);
        
        return Ok(new GenerateTokenResponse(tokenString));
    }
}