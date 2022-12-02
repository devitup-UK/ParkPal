namespace ParkPal.API.Models.Requests.Notification;

public class GetNotificationsRequest
{
    public NotificationFilters Filters { get; set; }
    
    public List<string> FavouriteIds { get; set; }
}