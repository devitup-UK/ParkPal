using Npgsql;
using ParkPal.Common.Models.Enums;
using ParkPal.PushEngine.Services;

namespace ParkPal.PushEngine.Workers;

public class AlertEvaluationWorker(ILogger<AlertEvaluationWorker> logger, IConfiguration config) : BackgroundService
{
    private readonly string _connString = config.GetConnectionString("DatabaseConnection")!;
    private readonly ApnsWrapper _apns = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("🚀 ParkPal Push Engine Started!");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessActiveAlertsAsync();
            }
            catch (Exception ex)
            {
                logger.LogError($"❌ Engine misfire: {ex.Message}");
            }

            // Rest for 60 seconds so we don't hammer the database
            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }

    private async Task ProcessActiveAlertsAsync()
    {
        await using var conn = new NpgsqlConnection(_connString);
        await conn.OpenAsync();

        // 1. Fetch Alerts (Now including AlertType!)
        const string fetchAlertsSql = @"
            SELECT 
                w.""AlertId"", w.""AttractionId"", w.""TargetWaitTime"", 
                d.""DeviceToken"", a.""Name"" AS ""AttractionName"",
                w.""AlertType"" -- ⭐️ Grab the Type!
            FROM ""Alerts"".""WaitTimeAlert"" w
            INNER JOIN ""Users"".""Device"" d ON w.""AppUserId"" = d.""AppUserId""
            INNER JOIN ""Static"".""Attraction"" a ON w.""AttractionId"" = a.""AttractionId""
            WHERE w.""IsActive"" = TRUE AND d.""DeviceToken"" IS NOT NULL;";

        await using var fetchCmd = new NpgsqlCommand(fetchAlertsSql, conn);
        await using var reader = await fetchCmd.ExecuteReaderAsync();

        // ⭐️ Updated Tuple to hold the AlertType (int)
        var alerts = new List<(Guid AlertId, string AttractionId, int TargetWaitTime, string DeviceToken, string AttractionName, int AlertType)>();
        var uniqueAttractionIds = new HashSet<string>(); 

        while (await reader.ReadAsync())
        {
            var attractionId = reader.GetString(1);
            alerts.Add((
                reader.GetGuid(0), attractionId, reader.GetInt32(2), 
                reader.GetString(3), reader.GetString(4), 
                reader.GetInt32(5) // ⭐️ Read the AlertType
            ));
            uniqueAttractionIds.Add(attractionId);
        }
        await reader.CloseAsync();

        if (!alerts.Any()) return;

        // 2. THE BATCH FETCH (Now grabbing Status too!)
        // ⭐️ Dictionary now holds a Tuple of (WaitTime, Status)
        var liveStates = new Dictionary<string, (int? WaitTime, int Status)>();
        
        const string fetchWaitsSql = @"
            SELECT ""AttractionId"", ""WaitTime"", ""Status"" 
            FROM ""Live"".""AttractionState"" 
            WHERE ""AttractionId"" = ANY(@ids);"; // ⭐️ Removed the 'WaitTime IS NOT NULL' filter so we can see closed rides!
            
        await using var waitCmd = new NpgsqlCommand(fetchWaitsSql, conn);
        waitCmd.Parameters.AddWithValue("ids", uniqueAttractionIds.ToArray());
        
        await using var waitReader = await waitCmd.ExecuteReaderAsync();
        while (await waitReader.ReadAsync())
        {
            int? waitTime = waitReader.IsDBNull(1) ? null : waitReader.GetInt32(1);
            int status = waitReader.IsDBNull(2) ? 0 : waitReader.GetInt32(2);
            
            liveStates[waitReader.GetString(0)] = (waitTime, status);
        }
        await waitReader.CloseAsync();
        
        var triggeredCounts = new Dictionary<string, int>();

        // 3. Evaluate the alerts purely in memory (Blazing fast!)
        // 3. Evaluate based on AlertType!
        foreach (var alert in alerts)
        {
            // If the ride isn't in our live table at all, skip it
            if (!liveStates.TryGetValue(alert.AttractionId, out var currentState)) continue;

            bool shouldTrigger = false;
            string pushMessage = "";

            switch (alert.AlertType)
            {
                case 0: // WaitTimeDropsBelow
                    if (currentState.WaitTime.HasValue && currentState.WaitTime.Value <= alert.TargetWaitTime)
                    {
                        shouldTrigger = true;
                        pushMessage = $"{alert.AttractionName} has dropped to {currentState.WaitTime.Value} minutes! Time to go!";
                    }
                    break;

                case 1: // WaitTimeExactly
                    if (currentState.WaitTime.HasValue && currentState.WaitTime.Value == alert.TargetWaitTime)
                    {
                        shouldTrigger = true;
                        pushMessage = $"{alert.AttractionName} is exactly at {currentState.WaitTime.Value} minutes!";
                    }
                    break;

                case 2: // RideReopens
                    if (currentState.Status == (int)ParkPalAttractionStatus.Operating)
                    {
                        shouldTrigger = true;
                        pushMessage = $"{alert.AttractionName} has just reopened! Go, go, go! ✨";
                    }
                    break;
            }

            // 4. Fire the push and disable!
            if (shouldTrigger)
            {
                if (!triggeredCounts.TryAdd(alert.AttractionName, 1))
                {
                    triggeredCounts[alert.AttractionName]++;
                }

                // Note: Update your ApnsWrapper method to accept a custom message string instead of building it inside the wrapper!
                await _apns.SendWaitTimeAlertAsync(alert.DeviceToken, "🎢 ParkPal Alert!", pushMessage);
            
                await DisableAlertAsync(alert.AlertId, conn);
            }
        }
        
        foreach (var summary in triggeredCounts)
        {
            logger.LogInformation($"🎯 {summary.Value} alerts triggered for {summary.Key}!");
        }
    }

    private async Task DisableAlertAsync(Guid alertId, NpgsqlConnection conn)
    {
        // ⭐️ Removed the phantom "TriggeredAt" column!
        const string updateSql = @"
            UPDATE ""Alerts"".""WaitTimeAlert"" 
            SET ""IsActive"" = FALSE 
            WHERE ""AlertId"" = @id;";
            
        await using var cmd = new NpgsqlCommand(updateSql, conn);
        cmd.Parameters.AddWithValue("id", alertId);
        
        await cmd.ExecuteNonQueryAsync();
    }
}