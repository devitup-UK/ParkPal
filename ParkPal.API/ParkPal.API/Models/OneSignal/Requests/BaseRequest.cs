using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ParkPal.API.Models.OneSignal.Requests;

public class BaseRequest
{
    [JsonPropertyName("app_id")]
    [JsonProperty(PropertyName = "app_id")]
    public string AppId { get; set; }
}