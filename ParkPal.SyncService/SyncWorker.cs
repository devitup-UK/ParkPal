using Npgsql;
using ParkPal.Common.Services;
using System.Text.Json;
using ParkPal.Common;
using ParkPal.Common.Data.Interfaces;
using ParkPal.Common.Models;
using ParkPal.Common.Services.Interfaces;

namespace ParkPal.SyncService;

public class SyncWorker : BackgroundService
{
    private readonly ILogger<SyncWorker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public SyncWorker(ILogger<SyncWorker> logger, IServiceProvider serviceProvider, IConfiguration config)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ParkPal Sync Engine online, buddy! 🚀");
        using PeriodicTimer timer = new(TimeSpan.FromMinutes(5));

        // ⭐️ The 'do' block runs immediately on startup!
        do
        {
            try
            {
                await PerformSyncAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sync failed! We'll try again in 5 mins.");
            }
        
            // ⭐️ The timer holds execution here for 5 minutes before looping back up
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }
    
    private string GenerateRuthlessKey(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return string.Empty;
        
        // ⭐️ Strip EVERYTHING except literal letters and numbers, then lowercase it
        var pureAlphanumeric = name.Where(char.IsLetterOrDigit).ToArray();
        return new string(pureAlphanumeric).ToLowerInvariant();
    }

    private async Task PerformSyncAsync(CancellationToken ct)
    {
        _logger.LogInformation("Starting Disney API Sync...");
    
        using var scope = _serviceProvider.CreateScope();
        var apiService = scope.ServiceProvider.GetRequiredService<IThemeParkService>();
        var dbRepository = scope.ServiceProvider.GetRequiredService<ISyncRepository>();

        var destinations = await apiService.GetDestinationsAsync();

        foreach (var destination in destinations)
        {
            await dbRepository.SyncStaticDestinationAsync(destination);

            foreach (var park in destination.Parks)
            {
                await dbRepository.SyncStaticParkAsync(park, destination.DestinationId);

                var parkWithLiveAttractions = await apiService.GetParkWithAttractionsAsync(park.ParkId);
                if (parkWithLiveAttractions?.Attractions == null) continue;
                
                // ⭐️ THE FRANKENSTEIN BOUNCER
                var deduplicatedAttractions = parkWithLiveAttractions.Attractions
                    .GroupBy(a => GenerateRuthlessKey(a.Name))
                    .Select(group => 
                    {
                        // 1. The Anchor: ALWAYS pick the lowest alphabetical ID. 
                        // This guarantees Postgres only ever sees ONE constant ID for this ride.
                        var anchor = group.OrderBy(a => a.AttractionId).First();
        
                        // 2. The Active: Find the clone that is actually open or has the best data
                        var active = group.OrderBy(a => a.Status.ToString() == "Operating" ? 0 : 1).First();
        
                        // 3. The Merge: Steal the live data from the active clone and give it to the anchor!
                        anchor.Status = active.Status;
        
                        // (Note: If you map wait times here too, copy them over!)
                        anchor.WaitTime = active.WaitTime; 
        
                        return anchor;
                    })
                    .ToList();

                foreach (var attraction in deduplicatedAttractions)
                {
                    
                    // 1. Serialize the state to a JSON string for our Data Lake
                    // 1. Serialize the state to a JSON string for our Data Lake
                    var rawJson = JsonSerializer.Serialize(attraction.LiveDataJson);

                    // 2. Execute the DB updates cleanly via the repository
                    await dbRepository.SyncStaticAttractionAsync(attraction, park.ParkId);
                    await dbRepository.SyncLiveStateAsync(attraction);

                    // ⭐️ THE NEW ROUTER:
                    if (attraction.EntityType == EntityType.SHOW)
                    {
                        // Shows go to the daily snapshot!
                        await dbRepository.SyncDailyShowScheduleAsync(attraction);
                    }
                    else 
                    {
                        // Rides/Restaurants go to the 5-minute Delta Engine!
                        await dbRepository.SyncHistoryAsync(attraction, rawJson);
                    }
                }
            }
        }
    
        _logger.LogInformation("Sync complete! Data locked in. 🔐");
    }
}