using Newtonsoft.Json;

namespace ParkPal.Common.API.Models.ThemeParkApi;

public class DestinationEntry
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }
    [JsonProperty(PropertyName = "slug")]
    public string Slug { get; set; }
    [JsonProperty(PropertyName = "parks")]
    public List<DestinationParkEntry> Parks { get; set; }
}