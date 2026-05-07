namespace ParkPal.Common.API.Models.Dtos;

public class SavedPlanDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime TripDate { get; set; }
    public string DestinationName { get; set; } = string.Empty;
    public string ParkName { get; set; } = string.Empty;
    public string ParkId { get; set; }
    public string? ArrivalTime { get; set; }
    public string? DepartureTime { get; set; }
    public int TotalActivities { get; set; } // Calculated field!
    public string ShareCode { get; set; } = string.Empty;
    public bool IsOwner { get; set; }
    
    public List<PlanItemDto> Items { get; set; } = new();
    public List<FlexibleItemDto> FlexibleItems { get; set; } = new();
}

public class FlexibleItemDto
{
    public Guid Id { get; set; }
    public string? AttractionId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string IconName { get; set; } = "ticket.fill"; 
    public string ColorHex { get; set; } = "#007AFF";
}