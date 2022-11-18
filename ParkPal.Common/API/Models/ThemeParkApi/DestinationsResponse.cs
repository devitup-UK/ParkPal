using Newtonsoft.Json;

namespace ParkPal.Common.API.Models.ThemeParkApi;

public class DestinationsResponse
{
    [JsonProperty(PropertyName = "destinations")]
    public List<DestinationEntry> Destinations { get; set; }
}