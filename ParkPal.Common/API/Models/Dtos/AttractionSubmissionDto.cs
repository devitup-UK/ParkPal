namespace ParkPal.Common.API.Models.Dtos;

public class AttractionSubmissionDto
{
    public string AttractionId { get; set; }
    public int ReportedStatus { get; set; } // 0 = Operating, 1 = Down, 2 = Closed
    public int? ReportedWaitTime { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}