using Newtonsoft.Json;
using ParkPal.Common.API.Models.ThemeParkApi.Enums;

namespace ParkPal.Common.API.Models.ThemeParkApi;

public class EntityLiveDataResponse
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }
    
    [JsonProperty(PropertyName = "entityType")]
    public EntityType EntityType { get; set; }
    
    [JsonProperty(PropertyName = "timezone")]
    public string TimeZone { get; set; }
    
    [JsonProperty(PropertyName = "liveData")]
    public List<EntityLiveData> LiveData { get; set; }
}