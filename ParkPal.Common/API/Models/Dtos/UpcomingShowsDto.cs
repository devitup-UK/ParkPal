namespace ParkPal.Common.API.Models.Dtos;

public class UpcomingShowsDto
{
    public string AttractionId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ParkTimezone { get; set; } = string.Empty;
    public List<DateTime> UpcomingShowtimes { get; set; } = new List<DateTime>();
    public bool IsContinuous { get; set; } = false;
}