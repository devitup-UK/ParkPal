using ParkPal.Common.API;
using ParkPal.Common.Data;
using ParkPal.Common.Data.Interfaces;
using ParkPal.Common.Services;
using ParkPal.Common.Services.Interfaces;
using ParkPal.SyncService;

var builder = Host.CreateApplicationBuilder(args);

// 1. Grab the Configuration
var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection")!;
var apiBaseUrl = builder.Configuration["Configuration:ThemeParkApiBaseUrl"];

// 2. Register the Database Repository
// We use AddScoped so that every time the 5-minute timer triggers and calls CreateScope(), 
// it gets a fresh repository and a fresh database connection lifecycle.
builder.Services.AddScoped<ISyncRepository>(sp => new SyncRepository(connectionString));

builder.Services.AddTransient<RateLimitingHandler>();

// 3. Register the API and Services
// This automatically provides an HttpClient to your ThemeParkApi
builder.Services.AddHttpClient<ThemeParkApi>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<RateLimitingHandler>();

builder.Services.AddScoped<IThemeParkService, ThemeParkService>();

// 4. Register the Background Worker
builder.Services.AddHostedService<SyncWorker>();

var host = builder.Build();
host.Run();