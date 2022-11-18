using Newtonsoft.Json;
using ParkPal.Common.API.Models.ThemeParkApi.Enums;

namespace ParkPal.Common.API.Models.ThemeParkApi;

public class EntityChild
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }
    [JsonProperty(PropertyName = "entityType")]
    public EntityType EntityType { get; set; }
    [JsonProperty(PropertyName = "slug")]
    public string Slug { get; set; }
}