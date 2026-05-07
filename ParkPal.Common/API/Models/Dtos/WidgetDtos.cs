namespace ParkPal.Common.API.Models.Dtos;

// 1. The Top Level Wrapper (Matches your JSON root)
public class CatalogResponseDto
{
    public List<DestinationCatalogDto> Destinations { get; set; } = new();
}

// 2. The Destination Layer
public class DestinationCatalogDto
{
    public string Name { get; set; } = string.Empty;
    public List<ParkCatalogDto> Parks { get; set; } = new();
}

// 3. The Park Layer
public class ParkCatalogDto
{
    public string ParkId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<AttractionCatalogDto> Attractions { get; set; } = new();
}

// 4. The Attraction Layer
public class AttractionCatalogDto
{
    public string AttractionId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}