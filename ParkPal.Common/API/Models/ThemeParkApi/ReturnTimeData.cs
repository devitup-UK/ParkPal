using Newtonsoft.Json;
using ParkPal.Common.API.Models.ThemeParkApi.Enums;

namespace ParkPal.Common.API.Models.ThemeParkApi;

public class ReturnTimeData
{
    [JsonProperty(PropertyName = "state")]
    public ReturnTimeState State { get; set; }
    
    [JsonProperty(PropertyName = "returnStart")]
    public DateTime? ReturnStart { get; set; }
    
    [JsonProperty(PropertyName = "returnEnd")]
    public DateTime? ReturnEnd { get; set; }
}