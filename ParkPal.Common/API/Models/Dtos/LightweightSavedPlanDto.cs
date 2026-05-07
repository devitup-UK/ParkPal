namespace ParkPal.Common.API.Models.Dtos;

public class LightweightSavedPlanDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime TripDate { get; set; }
    public string DestinationName { get; set; } = string.Empty;
    public string ParkName { get; set; } = string.Empty;
    public bool IsOwner { get; set; }
}