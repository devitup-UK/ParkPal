namespace ParkPal.API.Services.Interfaces;

public interface ITokenService
{
    bool Verify(string token);
    string GenerateToken(string appUserId);
}