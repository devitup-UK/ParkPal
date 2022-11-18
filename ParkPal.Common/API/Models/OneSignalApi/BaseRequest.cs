using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ParkPal.Common.API.Models.OneSignalApi;

public class BaseRequest
{
    [JsonPropertyName("app_id")]
    [JsonProperty(PropertyName = "app_id")]
    public string AppId { get; set; }
}