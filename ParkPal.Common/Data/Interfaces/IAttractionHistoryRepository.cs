using ParkPal.Common.API.Models.Dtos;

namespace ParkPal.Common.Data.Interfaces;

public interface IAttractionHistoryRepository
{
    Task<List<HistoricalWaitTimeBucketDto>> GetAveragesForDayAsync(List<string> attractionIds, DayOfWeek dayOfWeek);
}