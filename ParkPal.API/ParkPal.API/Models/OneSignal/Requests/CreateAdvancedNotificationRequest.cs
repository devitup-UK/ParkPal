using System.Text.Json.Serialization;

namespace ParkPal.API.Models.OneSignal.Requests;

public class CreateAdvancedNotificationRequest: CreateNotificationRequest
{
    [JsonPropertyName("subtitle")]
    public Dictionary<string, string> Subtitle { get; set; }
    
    [JsonPropertyName("template_id")]
    public string TemplateId { get; set; }
    
    [JsonPropertyName("content_available")]
    public bool ContentAvailable { get; set; }
    
    [JsonPropertyName("mutable_content")]
    public bool MutableContent { get; set; }
    
    [JsonPropertyName("target_content_identifier")]
    public string TargetContentIdentifier { get; set; }
}