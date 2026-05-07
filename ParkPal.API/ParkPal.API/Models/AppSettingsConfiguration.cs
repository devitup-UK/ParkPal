namespace ParkPal.API.Models;

public class AppSettingsConfiguration
{
    public string Secret  { get; set; }
    public string ThemeParkApiBaseUrl { get; set; }
    public string CdnBaseUrl { get; set; }
}