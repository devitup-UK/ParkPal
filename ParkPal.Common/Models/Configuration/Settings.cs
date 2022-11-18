using ParkPal.Common.API.Models.KeyVaultApi;

namespace ParkPal.Common.Models.Configuration;

public static class Settings
{
    public static List<Key> keys { get; set; }

    public static string? HostEnvironment;
    
    public static string? Secret => GetKeyValueByName("Secret");

    public static string? SQLConnectionString => GetKeyValueByName("SQLConnectionString");

    public static string ThemeParkWaitTimeUrl => GetKeyValueByName("ThemeParkWaitTimeUrl");

    public static string OneSignalApiUrl => GetKeyValueByName("OneSignalApiUrl");

    public static string OneSignalAppId => GetKeyValueByName("OneSignalAppId");

    private static string? GetKeyValueByName(string name)
    {
        HostEnvironment = name;
        return keys.FirstOrDefault(a => a.Name == name)?.Value;
    }
}