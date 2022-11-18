using Newtonsoft.Json;
using ParkPal.Common.API.Enums;
using ParkPal.Common.API.Models.OneSignalApi;
using ParkPal.Common.API.Models.ThemeParkApi;

namespace ParkPal.Common.API;

public class OneSignalApi: BaseApi
{
    public OneSignalApi(string baseUrl) : base(baseUrl)
    {
    }

    public CreateNotificationResponse? SendPushNotificationToPlayer(CreateNotificationRequest request)
    {
        SetHeader("accept", "application/json");
        // TODO - Move REST API key to a key vault or something, really need to come up with how to store things in a separate key vault service.
        SetHeader("Authorization", "NDM1NGE1NjctOWZlYi00YjI3LWE3OTMtNmRiZDUyMjlkZDU4");
        SetHeader("content-type", "application/json");
        
        string requestInJsonForm = JsonConvert.SerializeObject(request);
        CreateNotificationResponse? response = PostRequest<CreateNotificationResponse>($"/notifications", requestInJsonForm);
        return response;
    }
}