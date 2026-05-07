using ParkPal.Common.API.Enums;

namespace ParkPal.Common.API.Models;

public class CreateAlertRequest
{
    public string AppUserId { get; set; } = string.Empty;
    public string AttractionId { get; set; } = string.Empty;
    public AlertType AlertType { get; set; }
    public int TargetWaitTime { get; set; }
}