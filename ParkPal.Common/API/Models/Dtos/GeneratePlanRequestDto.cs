namespace ParkPal.Common.API.Models.Dtos;

public class GeneratePlanRequestDto
{
    public string AppUserId { get; set; } = string.Empty;
    public DateTime TripDate { get; set; }
    public string DestinationName { get; set; } = string.Empty;
    public string ParkId { get; set; }
    public string ArrivalTime { get; set; } 
    public string DepartureTime { get; set; }
    public List<string> MustDoAttractionIds { get; set; } = new();
    public List<string> NiceToHaveAttractionIds { get; set; } = new();
    public List<PlannedMealDto> PlannedMeals { get; set; } = new();
    public List<PlannedShowDto> SelectedShows { get; set; } = new();
}

public class PlannedMealDto
{
    public Guid Id { get; set; }
    public string RestaurantId { get; set; }
    public string RestaurantName { get; set; }
    public string Time { get; set; } 
}

public class PlannedShowDto
{
    public string ShowId { get; set; }
    public string? PreferredTime { get; set; }
    public List<string> ValidTimes { get; set; } = new();
}