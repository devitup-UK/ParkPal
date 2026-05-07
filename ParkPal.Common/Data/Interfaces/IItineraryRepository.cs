using ParkPal.Common.API.Models.Dtos;

namespace ParkPal.Common.Data.Interfaces;

public interface IItineraryRepository
{
    Task SavePlanAsync(string appUserId, SavedPlanDto plan);
    Task<List<SavedPlanDto>> GetUserPlansAsync(string appUserId);
    Task<SavedPlanDto> GetPlanByIdAsync(string planId, string appUserId);
    Task<SavedPlanDto?> GetPlanPreviewByShareCodeAsync(string shareCode);
    Task<bool> JoinPlanByShareCodeAsync(string appUserId, string shareCode);
    Task<bool> LeavePlanAsync(string appUserId, Guid planId);
    Task DeletePlanAsync(string appUserId, Guid planId);
    Task RenamePlanAsync(string appUserId, Guid planId, string newTitle);
}