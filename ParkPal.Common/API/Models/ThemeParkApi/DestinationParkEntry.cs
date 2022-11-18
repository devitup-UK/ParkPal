using Newtonsoft.Json;

namespace ParkPal.Common.API.Models.ThemeParkApi;

public class DestinationParkEntry
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }
}