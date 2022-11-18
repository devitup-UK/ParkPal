using ParkPal.API.Models.Enums;

namespace ParkPal.API.Models;

public class NotificationFilters
{
    public NotificationsFilterCriteria Criteria { get; set; }
    public NotificationsFilterType Type { get; set; }
    public NotificationsFilterSort Sort { get; set; }
    public string? ParkId { get; set; }
}