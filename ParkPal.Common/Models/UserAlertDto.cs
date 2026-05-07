using ParkPal.Common.API.Enums;

namespace ParkPal.Common.Models;

public class UserAlertDto
{
    public string AttractionId { get; set; } = string.Empty;
    public string AttractionName { get; set; } = string.Empty;
    
    // ⭐️ We send this so SwiftUI can group the list exactly like your screenshot!
    public string DestinationName { get; set; } = string.Empty; 
    
    public AlertType AlertType { get; set; }
    public int TargetWaitTime { get; set; }
    public bool IsActive { get; set; }
}