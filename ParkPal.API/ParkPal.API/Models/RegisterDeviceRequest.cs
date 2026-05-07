namespace ParkPal.API.Models;

public class RegisterDeviceRequest
{
    public string AppUserId { get; set; } = string.Empty;
    public string DeviceToken { get; set; } = string.Empty;
}