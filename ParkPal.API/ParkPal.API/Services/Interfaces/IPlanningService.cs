using ParkPal.Common.API.Models.Dtos;

namespace ParkPal.API.Services.Interfaces;


public interface IPlanningService
{
    Task SavePlanAsync(string appUserId, SavedPlanDto request);
    Task<SavedPlanDto> GenerateItineraryAsync(GeneratePlanRequestDto request);
    
}