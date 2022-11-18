using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;

namespace ParkPal.API.Models.OneSignal.Requests;

public class CreateNotificationRequest: BaseRequest
{
    [JsonPropertyName("include_player_ids")]
    [JsonProperty(PropertyName = "include_player_ids")]
    public string[] IncludePlayerIds { get; set; }
    
    [JsonPropertyName("contents")]
    [JsonProperty(PropertyName = "contents")]
    public Dictionary<string, string> Contents { get; set; }
    
    [JsonPropertyName("headings")]
    [JsonProperty(PropertyName = "headings")]
    public Dictionary<string, string> Headings { get; set; }
}