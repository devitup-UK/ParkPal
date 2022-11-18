using ParkPal.Common.Models.Database.Entities.Device;

namespace ParkPal.API.Services.Interfaces;

public interface ITokenService
{
    bool Verify(string token);
    Token? GetByToken(string token);
    public Token? Generate();
}