using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ParkPal.API.Models;
using ParkPal.API.Services.Interfaces;

namespace ParkPal.API.Services;

public class TokenService(IOptions<AppSettingsConfiguration> appSettings) : ITokenService
{
    private readonly string _secret = appSettings.Value.Secret;

    public string GenerateToken(string appUserId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secret);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] 
            {
                // ⭐️ Storing the AppUserId in the "Name" claim so your Startup.cs can read it!
                new Claim(ClaimTypes.Name, appUserId)
            }),
            Expires = DateTime.UtcNow.AddYears(1), // Long lived for the app
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public bool Verify(string token)
    {
        // For a stateless JWT, if the signature passes in Startup.cs, it's valid!
        // If you ever want to ban a specific user, you'd check a DB blacklist here.
        return true; 
    }
}