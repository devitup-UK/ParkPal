using ParkPal.Common.Models.Database.Entities.Notification.Enums;
using Type = ParkPal.Common.Models.Database.Entities.Notification.Enums.Type;

namespace ParkPal.API.Models.Requests.Notification;

public class CreateNotificationRequest
{
    public string? AttractionId { get; set; }
    public string ParkId { get; set; }
    public Type Type { get; set; }
    public int MinuteInterval { get; set; }
    public CriteriaType CriteriaType { get; set; }
    public int WaitTime { get; set; }
}