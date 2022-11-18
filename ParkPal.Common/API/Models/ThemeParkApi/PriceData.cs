using Newtonsoft.Json;

namespace ParkPal.Common.API.Models.ThemeParkApi;

public class PriceData
{
    [JsonProperty(PropertyName = "amount")]
    public int Amount { get; set; }
    
    [JsonProperty(PropertyName = "currency")]
    public string Currency { get; set; }
}