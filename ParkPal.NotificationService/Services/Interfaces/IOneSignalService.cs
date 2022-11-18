namespace ParkPal.NotificationService.Services.Interfaces;

public interface IOneSignalService
{
    void SendPushNotificationToPlayer(string title, string body, string playerId);
}