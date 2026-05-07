namespace ParkPal.Common.Data.Interfaces;

using ParkPal.Common.Models;

public interface ISyncRepository
{
    Task SyncStaticDestinationAsync(Destination destination);
    Task SyncStaticParkAsync(Park park, string destinationId);
    Task SyncStaticAttractionAsync(AttractionDto attractionDto, string parkId);
    Task SyncLiveStateAsync(AttractionDto attractionDto);
    
    // ⭐️ We pass the raw JSON string alongside the parsed model!
    Task SyncHistoryAsync(AttractionDto attractionDto, string rawJsonData);
    Task SyncDailyShowScheduleAsync(AttractionDto att);
}