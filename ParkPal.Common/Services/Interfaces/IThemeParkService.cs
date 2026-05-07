using ParkPal.Common.Models;

namespace ParkPal.Common.Services.Interfaces;

public interface IThemeParkService
{
    public Task<List<Destination>> GetDestinationsAsync();
    public Task<List<Park>> GetDestinationsParksAsync(string destinationId);
    public Task<Destination?> GetDestinationWithParksAsync(string destinationId);
    public Task<EntityLiveDataResponse?> GetParkWaitTimesAsync(string parkId);
    Task<EntityChildrenResponse?> GetParkChildrenAttractionsAsync(string parkId);
    public Task<Park?> GetParkWithAttractionsAsync(string parkId);
    public Task<List<AttractionDto>> GetParkAttractionsAsync(string parkId);
    public Task<int?> GetAttractionWaitTimeAsync(string parkId, string attractionId);
}