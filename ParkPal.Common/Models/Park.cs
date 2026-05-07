namespace ParkPal.Common.Models;

public class Park
{
    public string ParkId { get; set; }
    public string Name { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageBlurHash { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Timezone { get; set; }

    public List<AttractionDto> Attractions { get; set; }

    public Park(string parkId, string name)
    {
        ParkId = parkId;
        Name = name;
        Attractions = new List<AttractionDto>();
    }
    
    
}