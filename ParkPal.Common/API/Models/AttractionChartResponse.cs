namespace ParkPal.Common.API.Models;

public class AttractionChartResponse
{
    public string TimeZone { get; set; } = "UTC";
    public List<WaitTimeTrendDto> HistoricalData { get; set; } = [];
    public List<WaitTimeTrendDto> TodayData { get; set; } = [];
}