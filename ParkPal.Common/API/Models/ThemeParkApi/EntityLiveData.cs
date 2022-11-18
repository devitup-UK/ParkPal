
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ParkPal.Common.API.Models.ThemeParkApi.Enums;

namespace ParkPal.Common.API.Models.ThemeParkApi;

public class EntityLiveData
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }
    
    [JsonProperty(PropertyName = "entityType")]
    public EntityType EntityType { get; set; }
    
    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "status")]
    public LiveStatusType Status { get; set; }
    
    [JsonProperty(PropertyName = "lastUpdated")]
    public DateTime LastUpdated { get; set; }
    
    [JsonProperty(PropertyName = "queue")]
    public LiveQueue? Queue { get; set; }
    
    [JsonProperty(PropertyName = "showtimes")]
    public List<LiveShowTime> ShowTimes { get; set; }
}