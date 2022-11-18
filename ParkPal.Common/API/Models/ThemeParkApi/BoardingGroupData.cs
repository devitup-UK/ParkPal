using Newtonsoft.Json;
using ParkPal.Common.API.Models.ThemeParkApi.Enums;

namespace ParkPal.Common.API.Models.ThemeParkApi;

public class BoardingGroupData
{
    [JsonProperty(PropertyName = "allocationStatus")]
    public BoardingGroupState AllocationStatus { get; set; }
    
    [JsonProperty(PropertyName = "currentGroupStart")]
    public DateTime? CurrentGroupStart { get; set; }
    
    [JsonProperty(PropertyName = "currentGroupEnd")]
    public DateTime? CurrentGroupEnd { get; set; }
    
    [JsonProperty(PropertyName = "nextAllocationTime")]
    public DateTime? NextAllocationTime { get; set; }
    
    [JsonProperty(PropertyName = "estimatedWait")]
    public int? EstimatedWait { get; set; }
}