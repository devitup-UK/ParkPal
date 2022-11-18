using Newtonsoft.Json;

namespace ParkPal.Common.API.Models.ThemeParkApi;

public class LiveShowTime
{
    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; }
    
    [JsonProperty(PropertyName = "startTime")]
    public DateTime StartTime { get; set; }
    
    [JsonProperty(PropertyName = "endTime")]
    public DateTime EndTime { get; set; }
}