using ParkPal.Common.API.Models;
using ParkPal.Common.API.Models.Dtos;
using ParkPal.Common.Models;

namespace ParkPal.Common.Data.Interfaces;

public interface IParkRepository
{
    Task<List<Destination>> GetActiveDestinationsAsync();
    Task<Destination?> GetDestinationWithParksAsync(string destinationId);
    Task<Park?> GetParkDataAsync(string parkId);
    Task<ParkLocationDto?> GetParkLocationForAttractionAsync(string attractionId);
    Task<List<BaseAttractionDto>> GetParkAttractionsAsync(string parkId);
    Task<Park?> GetParkWithLiveAttractionsAsync(string parkId);
    Task<AttractionChartResponse> GetAttractionChartDataAsync(string attractionId);
    Task<CatalogResponseDto> GetWidgetEntityCatalogByTypeAsync(string entityType);
    Task<LiveAttractionStatusDto?> GetLiveAttractionStatusAsync(string attractionId);
    Task<UpcomingShowsDto?> GetUpcomingShowtimesAsync(string attractionId);
    Task<List<RestaurantDto>> GetRestaurantsForParkAsync(string parkId);
    Task<List<PlannerShowDto>> GetShowsForParkAsync(string parkId, DateTime requestedDate);
    Task<List<BaseAttractionDto>> GetAttractionsWithLocationsForPark(string parkId);
    Task<List<string>> GetShowtimesAsync(string attractionId, DateTime requestedDate);
}