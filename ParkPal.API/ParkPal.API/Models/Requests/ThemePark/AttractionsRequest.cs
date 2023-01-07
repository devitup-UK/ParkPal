namespace ParkPal.API.Models.Requests.ThemePark;

public class AttractionsRequest
{
    public WaitTimeFilters Filters { get; set; }
    public List<string> FavouriteIds { get; set; }
}