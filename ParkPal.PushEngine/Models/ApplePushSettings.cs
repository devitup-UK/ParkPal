namespace ParkPal.PushEngine.Models;

public class ApplePushSettings
{
    public string P8PrivateKey { get; set; } = string.Empty;
    public string P8KeyId { get; set; } = string.Empty;
    public string TeamId { get; set; } = string.Empty;
    public string AppBundleId { get; set; } = string.Empty;
    
    public bool UseProductionServers { get; set; } = false; 
}