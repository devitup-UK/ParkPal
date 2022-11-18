using ParkPal.Common.Models.Database.Entities.Notification.Enums;

namespace ParkPal.API.Models.Requests.Notification;

public class CreateNotificationRequest
{
    public string AttractionId { get; set; }
    public string ParkId { get; set; }
    public int MinuteInterval { get; set; }
    public CriteriaType CriteriaType { get; set; }
    public int WaitTime { get; set; }
}