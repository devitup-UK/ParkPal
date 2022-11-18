using ParkPal.Common.API;
using ParkPal.Common.API.Models.OneSignalApi;
using ParkPal.Common.Models.Configuration;
using ParkPal.NotificationService.Services.Interfaces;
using RestSharp;

namespace ParkPal.NotificationService.Services;

public class OneSignalService: IOneSignalService
{
    public void SendPushNotificationToPlayer(string title, string body, string playerId)
    {
        Dictionary<string, string> headings = new() { { "en", title } };
        Dictionary<string, string> contents = new() { { "en", body } };

        CreateNotificationRequest request = new CreateNotificationRequest()
        {
            AppId = Settings.OneSignalAppId,
            IncludePlayerIds = new[] { playerId },
            Contents = contents,
            Headings = headings
        };

        OneSignalApi api = new OneSignalApi(Settings.OneSignalApiUrl);
        CreateNotificationResponse? response = api.SendPushNotificationToPlayer(request);

    }

}