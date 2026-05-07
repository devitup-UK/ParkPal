namespace ParkPal.Common.API.Models.Dtos;

public class LiveAttractionStatusDto
{
    public string AttractionId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int? WaitTime { get; set; }
    public int Status { get; set; } // 0: Operating, 1: Down, 2: Closed
}