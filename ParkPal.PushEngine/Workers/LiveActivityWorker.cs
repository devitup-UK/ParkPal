using Npgsql;
using ParkPal.Common.Models.Enums;
using ParkPal.PushEngine.Services;

namespace ParkPal.PushEngine.Workers;

public class LiveActivityWorker(ILogger<LiveActivityWorker> logger, IConfiguration config) : BackgroundService
{
    private readonly string _connString = config.GetConnectionString("DatabaseConnection")!;
    private readonly ApnsWrapper _apns = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("🏝️ ParkPal Live Activity Engine Started!");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessLiveActivitiesAsync();
            }
            catch (Exception ex)
            {
                // ⭐️ Dig into the InnerException!
                var realError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                logger.LogError($"❌ Live Activity Engine misfire: {realError}");
                
                // If it's a socket exception, this will print the exact network code
                if (ex.InnerException?.InnerException != null)
                {
                    logger.LogError($"🔍 Deep inner: {ex.InnerException.InnerException.Message}");
                }
            }

            // Rest for 60 seconds (Independent of the Alert worker!)
            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }

    private async Task ProcessLiveActivitiesAsync()
    {
        await using var conn = new NpgsqlConnection(_connString);
        await conn.OpenAsync();

        // 1. Grab active tokens and join with live ride state
        const string fetchSql = @"
            SELECT 
                l.""Id"", l.""PushToken"", l.""LastSentWaitTime"", l.""LastSentStatus"",
                a.""WaitTime"", a.""Status""
            FROM ""Alerts"".""LiveActivityMonitor"" l
            INNER JOIN ""Live"".""AttractionState"" a ON l.""AttractionId"" = a.""AttractionId""
            WHERE l.""ExpiresAt"" > CURRENT_TIMESTAMP;";

        await using var fetchCmd = new NpgsqlCommand(fetchSql, conn);
        await using var reader = await fetchCmd.ExecuteReaderAsync();

        var updatesToSend = new List<(Guid Id, string Token, int? NewWait, int NewStatus)>();

        while (await reader.ReadAsync())
        {
            var id = reader.GetGuid(0);
            var token = reader.GetString(1);
            
            int? lastWait = reader.IsDBNull(2) ? null : reader.GetInt32(2);
            int? lastStatus = reader.IsDBNull(3) ? null : reader.GetInt32(3);
            
            int? currentWait = reader.IsDBNull(4) ? null : reader.GetInt32(4);
            int currentStatus = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);

            // ⭐️ Rate Limit Saver: Only trigger if the data changed
            if (lastWait != currentWait || lastStatus != currentStatus)
            {
                updatesToSend.Add((id, token, currentWait, currentStatus));
            }
        }
        await reader.CloseAsync();

        if (!updatesToSend.Any()) return;

        logger.LogInformation($"📱 Pushing {updatesToSend.Count} Live Activity updates to Apple...");

        const string updateTrackerSql = @"
            UPDATE ""Alerts"".""LiveActivityMonitor"" 
            SET ""LastSentWaitTime"" = @wait, ""LastSentStatus"" = @status
            WHERE ""Id"" = @id;";

        foreach (var update in updatesToSend)
        {
            string statusString = update.NewStatus == (int)ParkPalAttractionStatus.Operating ? "Operating" : "Delayed/Closed";
            
            bool success = await _apns.SendLiveActivityUpdateAsync(update.Token, update.NewWait ?? 0, statusString);

            if (success)
            {
                await using var updateCmd = new NpgsqlCommand(updateTrackerSql, conn);
                updateCmd.Parameters.AddWithValue("wait", (object?)update.NewWait ?? DBNull.Value);
                updateCmd.Parameters.AddWithValue("status", update.NewStatus);
                updateCmd.Parameters.AddWithValue("id", update.Id);
                await updateCmd.ExecuteNonQueryAsync();
            }
        }
    }
}