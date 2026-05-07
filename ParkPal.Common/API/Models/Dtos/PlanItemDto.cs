namespace ParkPal.Common.API.Models.Dtos;

public class PlanItemDto
{
    public Guid Id { get; set; }
    public string? AttractionId { get; set; }
    public string? Time { get; set; } = string.Empty; // e.g., "09:00 AM"
    
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    
    // We can let the C# API dictate the UI theme based on the ItemType!
    public string Icon { get; set; } = "ticket.fill"; 
    public string ColorHex { get; set; } = "#007AFF";
}