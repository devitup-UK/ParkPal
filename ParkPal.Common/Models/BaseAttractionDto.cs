using ParkPal.Common.Models.Enums;

namespace ParkPal.Common.Models;

public class BaseAttractionDto
{
    public string AttractionId { get; set; }
    public string Name { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public BaseAttractionDto(string attractionId, string name)
    {
        AttractionId = attractionId;
        Name = name;
    }
}