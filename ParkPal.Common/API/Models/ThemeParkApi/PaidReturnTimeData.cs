using Newtonsoft.Json;
using ParkPal.Common.API.Models.ThemeParkApi.Enums;

namespace ParkPal.Common.API.Models.ThemeParkApi;

public class PaidReturnTimeData
{
    [JsonProperty(PropertyName = "state")]
    public ReturnTimeState State { get; set; }

    [JsonProperty(PropertyName = "returnStart")]
    public DateTime? ReturnStart { get; set; }

    [JsonProperty(PropertyName = "returnEnd")]
    public DateTime? ReturnEnd { get; set; }
    
    [JsonProperty(PropertyName = "price")]
    public PriceData Price { get; set; }
}