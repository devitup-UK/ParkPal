using ParkPal.Common.API;
using ParkPal.Common.Database.Contexts;
using ParkPal.Common.Logging.Providers;
using ParkPal.Common.Models.Configuration;
using ParkPal.Common.Services;
using ParkPal.Common.Services.Interfaces;
using ParkPal.NotificationService.BackgroundServices;
using ParkPal.NotificationService.Services;
using ParkPal.NotificationService.Services.Interfaces;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(loggerFactory => loggerFactory.AddProvider(new DbLoggerProvider(new ConfigurationManager().AddJsonFile("appsettings.json").Build())))
    .ConfigureServices((hostContext, services) =>
    {
        // Build up our configuration.
        IConfiguration configuration = hostContext.Configuration;
        IConfigurationSection appSettingsSection = configuration.GetSection("AppSettings");
        services.Configure<AppSettings>(appSettingsSection);
        
        // Build up our global settings file.
        AppSettings appSettings = appSettingsSection.Get<AppSettings>();
        ConfigurationService configurationService = new ConfigurationService(appSettings);
        configurationService.ConfigureSettings();

        services.AddDbContext<DatabaseContext>();
        services.AddScoped<IThemeParkService, ThemeParkService>();
        services.AddScoped<IOneSignalService, OneSignalService>();
        services.AddHostedService<TimerService>();
    })
    .UseWindowsService()
    .Build();

await host.RunAsync();