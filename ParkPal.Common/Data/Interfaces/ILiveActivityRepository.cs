using ParkPal.Common.API.Models.Dtos;

namespace ParkPal.Common.Data.Interfaces;

public interface ILiveActivityRepository
{
    Task RegisterMonitorAsync(string appUserId, RegisterLiveActivityRequest request);
    Task RemoveMonitorAsync(string appUserId, string attractionId);
}