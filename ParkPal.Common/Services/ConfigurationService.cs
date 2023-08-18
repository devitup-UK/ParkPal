using Microsoft.Extensions.Options;
using ParkPal.Common.API;
using ParkPal.Common.API.Models.KeyVaultApi;
using ParkPal.Common.Models.Configuration;

namespace ParkPal.Common.Services;

public class ConfigurationService
{
    private readonly KeyVaultApi _api;
    private readonly AppSettings _appSettings;

    // Call the KeyVault API and map settings to Settings file.
    public ConfigurationService(AppSettings appSettings)
    {
        _appSettings = appSettings;
        _api = new KeyVaultApi(_appSettings.KeyVaultUrl);
    }

    public void ConfigureSettings()
    {
        Settings.Keys = _api.GetAllKeys(_appSettings.HostEnvironment, _appSettings.ClientToken);
    }
}