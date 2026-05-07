using ParkPal.Common.API.Models.Dtos;

namespace ParkPal.Common.Data.Interfaces;

public interface IUsersRepository
{
    Task RegisterDeviceHandshakeAsync(UserRegistrationDto registration);
    Task IncreaseUserTrustScoreAsync(string userId);
    Task DecreaseUserTrustScoreAsync(string userId, int penalty);
    Task<UserProfileDto?> GetProfileAsync(string userId);
}