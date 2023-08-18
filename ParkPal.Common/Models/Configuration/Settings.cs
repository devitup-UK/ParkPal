using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using ParkPal.Common.API.Models.KeyVaultApi;

namespace ParkPal.Common.Models.Configuration;

public static class Settings
{
    public static List<Key> Keys = new List<Key>();

    public static string? HostEnvironment;
    
    public static string Secret => GetKeyValueByName("Secret");

    public static string SQLConnectionString => GetKeyValueByName("SQLConnectionString");

    public static string ThemeParkWaitTimeUrl => GetKeyValueByName("ThemeParkWaitTimeUrl");

    public static string OneSignalApiUrl => GetKeyValueByName("OneSignalApiUrl");

    public static string OneSignalAppId => GetKeyValueByName("OneSignalAppId");

    private static string GetKeyValueByName(string name)
    {
        HostEnvironment = name;
        Key? key = Keys.FirstOrDefault(a => a.Name == name);

        if (key != null)
        {
            return key.Value;
        }

        return "";
    }
}