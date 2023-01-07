using ParkPal.Common.Models.Database.Entities.Notification.Enums;

namespace ParkPal.API.Models.Requests.Notification;

public class EditNotificationRequest
{
    public int NotificationId { get; set; }
    public CriteriaType CriteriaType { get; set; }
    public int WaitTime { get; set; }
}