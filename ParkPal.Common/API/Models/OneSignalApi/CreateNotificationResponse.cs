using Newtonsoft.Json;

namespace ParkPal.Common.API.Models.OneSignalApi;

public class CreateNotificationResponse
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    [JsonProperty(PropertyName = "recipients")]
    public int Recipients { get; set; }
    [JsonProperty(PropertyName = "external_id")]
    public string? ExternalId { get; set; }
}