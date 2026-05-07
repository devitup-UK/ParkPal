using System.Text.Json.Serialization;
using ParkPal.Common.Models.Enums;

namespace ParkPal.Common.Models;

public class AttractionDto
{
    public string AttractionId { get; set; }
    public string? ExternalId { get; set; }
    public string Name { get; set; }
    public ParkPalAttractionStatus Status { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EntityType EntityType { get; set; }

    public bool Thrill { get; set; }

    public bool Hidden { get; set; }

    public int? WaitTime { get; set; }
    public DateTimeOffset? LastUpdated { get; set; }
    public int? SingleRiderWaitTime { get; set; }
    public DateTimeOffset? LightningLaneReturnStart { get; set; }
    public double? LightningLanePrice { get; set; }
    public bool IsVirtualQueueOnly { get; set; }
    public bool HasActiveAlert { get; set; }
    public int? CommunityWaitTime { get; set; }
    public DateTimeOffset? LastCommunityUpdate { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    [JsonIgnore]
    public string? ShowtimesJson { get; set; }
    public List<ShowtimeDto>? Showtimes { get; set; }
    public string? LiveDataJson { get; set; }

    public AttractionDto(string attractionId, string name, EntityType entityType, ParkPalAttractionStatus status)
    {
        AttractionId = attractionId;
        Name = name;
        EntityType = entityType;
        Status = status;
    }
}