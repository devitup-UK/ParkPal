using ParkPal.Common.API.Models;
using ParkPal.Common.Models;

namespace ParkPal.Common.Data.Interfaces;

public interface IAlertRepository
{
    Task<bool> UpsertAlertAsync(CreateAlertRequest request);
    Task<List<UserAlertDto>> GetUserAlertsAsync(string appUserId);
    Task<bool> DeleteAlertAsync(string appUserId, string attractionId);
    Task<bool> ToggleAlertStatusAsync(string appUserId, string attractionId, bool isActive);
}