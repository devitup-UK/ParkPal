using Newtonsoft.Json;

namespace ParkPal.Common.API.Models.ThemeParkApi;

public class LiveQueue
{
    [JsonProperty(PropertyName = "STANDBY")]
    public WaitTimeData? STANDBY { get; set; }
    
    // [JsonProperty(PropertyName = "SINGLE_RIDER")]
    // public WaitTimeData SingleRider { get; set; }
    //
    // [JsonProperty(PropertyName = "RETURN_TIME")]
    // public ReturnTimeData ReturnTime { get; set; }
    //
    // [JsonProperty(PropertyName = "PAID_RETURN_TIME")]
    // public PaidReturnTimeData PaidReturnTime { get; set; }
    //
    // [JsonProperty(PropertyName = "BOARDING_GROUP")]
    // public BoardingGroupData BoardingGroup { get; set; }
}