using Newtonsoft.Json;

namespace ParkPal.Common.API.Models.ThemeParkApi;

public class WaitTimeData
{
    [JsonProperty(PropertyName = "waitTime")]
    public int? WaitTime { get; set; }
}