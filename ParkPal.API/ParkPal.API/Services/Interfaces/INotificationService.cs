using ParkPal.API.Models.OneSignal.Requests;
using ParkPal.API.Models.Responses;
using ParkPal.Common.Models.Database.Entities.Notification;
using ParkPal.Common.Models.Database.Entities.Notification.Enums;
using Type = ParkPal.Common.Models.Database.Entities.Notification.Enums.Type;

namespace ParkPal.API.Services.Interfaces;

public interface INotificationService
{
    public List<Notification> GetAllNotifications(string token);
    public Item? GetNotification(string playerId, string attractionId, string parkId);
    public Item? CreateNotification(string token, Type type, string attractionId, string parkId,
        CriteriaType criteriaType, int waitTime, int minuteInterval = 5);

    public Item? EditNotification(int notificationId,
        CriteriaType criteriaType,
        int waitTime);

    public Item? DisableNotification(int notificationId);
    public Item? EnableNotification(int notificationId);
    public Item? SetEnabledFlag(int notificationId, bool enabled);

    public bool DeleteNotification(int notificationId);
}