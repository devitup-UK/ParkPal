namespace ParkPal.Common.API.Models.Dtos;

public class HistoricalWaitTimeBucketDto
{
    public string AttractionId { get; set; }
    public string AttractionName { get; set; } = string.Empty;
    public TimeSpan BucketTime { get; set; }
    public int AverageWaitTime { get; set; }
}