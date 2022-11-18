using System;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;
using ParkPal.Common.API;
using ParkPal.Common.API.Models.ThemeParkApi;
using ParkPal.Common.Models.Configuration;
using ParkPal.Common.Services;

namespace NotificationServiceTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        IConfiguration configuration = InitConfiguration();
        IConfigurationSection appSettingsSection = configuration.GetSection("AppSettings");
        AppSettings appSettings = appSettingsSection.Get<AppSettings>();
        
        // Build up our global settings.
        ConfigurationService configurationService = new ConfigurationService(appSettings);
        configurationService.ConfigureSettings();
    }
    
    public static IConfiguration InitConfiguration()
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        return config;
    }

    [Test]
    public void ThemeParkServiceTest()
    {
        ThemeParkApi api = new(Settings.ThemeParkWaitTimeUrl);
        EntityLiveDataResponse response = api.GetWaitTimes("magickingdompark");
        Assert.IsNotNull(response);
    }
}