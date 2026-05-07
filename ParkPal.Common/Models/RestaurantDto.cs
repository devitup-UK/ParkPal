using System.Text.Json.Serialization;
using ParkPal.Common.Models.Enums;

namespace ParkPal.Common.Models;

public class RestaurantDto(string attractionId, string name)
{
    public string AttractionId { get; set; } = attractionId;
    public string Name { get; set; } = name;
}