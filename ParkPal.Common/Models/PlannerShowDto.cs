namespace ParkPal.Common.Models;

public class PlannerShowDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<string> Showtimes { get; set; } = new();
}