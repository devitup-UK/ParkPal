using Npgsql;
using NpgsqlTypes;
using ParkPal.Common.Data.Interfaces; // ⭐️ Needed for JSONB
using ParkPal.Common.Models;

namespace ParkPal.Common.Data;

public class SyncRepository(string connectionString) : ISyncRepository
{
    // ... (SyncDestination, SyncPark, SyncStatic logic goes here, using the SQL we wrote earlier) ...
    public async Task SyncStaticDestinationAsync(Destination destination)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();
        
        // ⭐️ "ON CONFLICT DO NOTHING" means if it's already there, Postgres just ignores the command! Super fast.
        const string sql = @"
        INSERT INTO ""Static"".""Destination"" (""DestinationId"", ""Name"", ""TimeZone"", ""Longitude"", ""Latitude"") 
        VALUES (@id, @name, @timezone, @longitude, @latitude) 
        ON CONFLICT (""DestinationId"") DO UPDATE 
        SET ""Name"" = EXCLUDED.""Name"", 
            ""TimeZone"" = EXCLUDED.""TimeZone"",
            ""Longitude"" = EXCLUDED.""Longitude"",
            ""Latitude"" = EXCLUDED.""Latitude"";";
            
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("id", destination.DestinationId);
        cmd.Parameters.AddWithValue("name", destination.Name);
        cmd.Parameters.AddWithValue("timezone", destination.Timezone ?? "UTC");
        cmd.Parameters.AddWithValue("longitude", destination.Longitude ?? null);
        cmd.Parameters.AddWithValue("latitude", destination.Latitude ?? null);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task SyncStaticParkAsync(Park park, string destinationId)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();
        
        const string sql = @"
        INSERT INTO ""Static"".""Park"" 
            (""ParkId"", ""Name"", ""DestinationId"", ""Latitude"", ""Longitude"") 
        VALUES 
            (@id, @name, @destinationId, @lat, @lng) 
        ON CONFLICT (""ParkId"") DO UPDATE SET 
            ""Name"" = EXCLUDED.""Name"",
            ""Latitude"" = EXCLUDED.""Latitude"",
            ""Longitude"" = EXCLUDED.""Longitude"";";
            
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("id", park.ParkId);
        cmd.Parameters.AddWithValue("destinationId", destinationId);
        cmd.Parameters.AddWithValue("name", park.Name);
        
        // ⭐️ Safely pass the coordinates (Handle nulls if the API missed them)
        cmd.Parameters.AddWithValue("lat", park.Latitude ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("lng", park.Longitude ?? (object)DBNull.Value);
        
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task SyncStaticAttractionAsync(AttractionDto att, string parkId)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();
        
        // ⭐️ Note: We don't overwrite IsThrill/IsHidden if it exists, so your manual DB edits are safe!
        const string sql = @"
            INSERT INTO ""Static"".""Attraction"" (""AttractionId"", ""ParkId"", ""Name"", ""IsThrill"", ""IsHidden"", ""ExternalId"", ""EntityType"", ""Longitude"", ""Latitude"") 
            VALUES (@id, @parkId, @name, @thrill, @hidden, @externalId, @entityType, @longitude, @latitude) 
            ON CONFLICT (""AttractionId"") DO UPDATE SET
            ""Name"" = EXCLUDED.""Name"",
            ""ExternalId"" = EXCLUDED.""ExternalId"",
            ""EntityType"" = EXCLUDED.""EntityType"",
            ""Longitude"" = EXCLUDED.""Longitude"",
            ""Latitude"" = EXCLUDED.""Latitude"";";
            
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("id", att.AttractionId);
        cmd.Parameters.AddWithValue("parkId", parkId);
        cmd.Parameters.AddWithValue("name", att.Name);
        cmd.Parameters.AddWithValue("thrill", att.Thrill);
        cmd.Parameters.AddWithValue("hidden", att.Hidden);
        cmd.Parameters.AddWithValue("externalId", att.ExternalId == null ? DBNull.Value : att.ExternalId);
        cmd.Parameters.AddWithValue("entityType", att.EntityType.ToString());
        cmd.Parameters.AddWithValue("longitude", att.Longitude == null ? DBNull.Value: att.Longitude);
        cmd.Parameters.AddWithValue("latitude", att.Latitude == null ? DBNull.Value :  att.Latitude);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task SyncLiveStateAsync(AttractionDto att)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        const string sql = @"
            INSERT INTO ""Live"".""AttractionState"" 
            (""AttractionId"", ""WaitTime"", ""Status"", ""LastUpdated"", ""SingleRiderWaitTime"", ""LightningLaneReturnStart"", ""LightningLanePrice"", ""IsVirtualQueueOnly"", ""Showtimes"")
            VALUES (@id, @wait, @status, @lastUpdated, @singleRider, @llStart, @llPrice, @vqOnly, @showtimes)
            ON CONFLICT (""AttractionId"") DO UPDATE SET
                ""WaitTime"" = EXCLUDED.""WaitTime"",
                ""Status"" = EXCLUDED.""Status"",
                ""LastUpdated"" = EXCLUDED.""LastUpdated"",
                ""SingleRiderWaitTime"" = EXCLUDED.""SingleRiderWaitTime"",
                ""LightningLaneReturnStart"" = EXCLUDED.""LightningLaneReturnStart"",
                ""LightningLanePrice"" = EXCLUDED.""LightningLanePrice"",
                ""IsVirtualQueueOnly"" = EXCLUDED.""IsVirtualQueueOnly"",
                ""Showtimes"" = EXCLUDED.""Showtimes"";";

        await using var cmd = new NpgsqlCommand(sql, conn);
        
        var safeLastUpdated = att.LastUpdated?.ToUniversalTime() ?? DateTimeOffset.UtcNow;
        object safeLlStart = att.LightningLaneReturnStart.HasValue 
            ? att.LightningLaneReturnStart.Value.ToUniversalTime() 
            : DBNull.Value;
        
        cmd.Parameters.AddWithValue("id", att.AttractionId);
        cmd.Parameters.AddWithValue("wait", att.WaitTime ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("status", (int)att.Status);
        cmd.Parameters.AddWithValue("lastUpdated", safeLastUpdated);
        cmd.Parameters.AddWithValue("singleRider", att.SingleRiderWaitTime ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("llStart", safeLlStart);
        cmd.Parameters.AddWithValue("llPrice", att.LightningLanePrice ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("vqOnly", att.IsVirtualQueueOnly);
        cmd.Parameters.Add(new NpgsqlParameter("showtimes", NpgsqlDbType.Jsonb) { Value = att.ShowtimesJson == null ? DBNull.Value : att.ShowtimesJson });
        
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task SyncHistoryAsync(AttractionDto att, string rawJsonData)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();
        var now = DateTimeOffset.UtcNow;

        // 1. Fetch the absolute latest history record for this ride
        const string checkSql = @"
            SELECT ""HistoryId"", ""WaitTime"", ""Status"" 
            FROM ""History"".""Attraction"" 
            WHERE ""AttractionId"" = @id 
            ORDER BY ""StartTime"" DESC LIMIT 1;";
            
        await using var checkCmd = new NpgsqlCommand(checkSql, conn);
        checkCmd.Parameters.AddWithValue("id", att.AttractionId);
        
        await using var reader = await checkCmd.ExecuteReaderAsync();
        
        bool hasHistory = await reader.ReadAsync();
        int? lastWait = hasHistory ? reader.IsDBNull(1) ? null : reader.GetInt32(1) : null;
        int? lastStatus = hasHistory ? reader.GetInt32(2) : null;
        int? historyId = hasHistory ? reader.GetInt32(0) : null;
        
        await reader.CloseAsync();

        // 2. The Delta Engine 🧠
        int currentStatusInt = (int)att.Status;

        if (hasHistory && lastWait == att.WaitTime && lastStatus == currentStatusInt)
        {
            // State hasn't changed! Just update the LastSeenTime to save disk space.
            const string updateSql = @"UPDATE ""History"".""Attraction"" SET ""LastSeenTime"" = @now WHERE ""HistoryId"" = @hid;";
            await using var updateCmd = new NpgsqlCommand(updateSql, conn);
            updateCmd.Parameters.AddWithValue("now", now);
            updateCmd.Parameters.AddWithValue("hid", historyId!);
            await updateCmd.ExecuteNonQueryAsync();
        }
        else
        {
            // State CHANGED (or is brand new)! Insert a fresh history block, including the JSON Data Lake.
            const string insertSql = @"
                INSERT INTO ""History"".""Attraction"" 
                (""AttractionId"", ""WaitTime"", ""Status"", ""StartTime"", ""LastSeenTime"", ""RawData"")
                VALUES (@id, @wait, @status, @now, @now, @rawJson);";
                
            await using var insertCmd = new NpgsqlCommand(insertSql, conn);
            insertCmd.Parameters.AddWithValue("id", att.AttractionId);
            
            // ⭐️ THE FIX: Save the true state. If it's null, save DBNull!
            insertCmd.Parameters.AddWithValue("wait", att.WaitTime.HasValue ? att.WaitTime.Value : DBNull.Value); 
            
            insertCmd.Parameters.AddWithValue("status", currentStatusInt);
            insertCmd.Parameters.AddWithValue("now", now);
            insertCmd.Parameters.Add(new NpgsqlParameter("rawJson", NpgsqlDbType.Jsonb) { Value = rawJsonData });
            
            await insertCmd.ExecuteNonQueryAsync();
        }
    }
    
    public async Task SyncDailyShowScheduleAsync(AttractionDto att)
    {
        // Don't bother if there are no showtimes!
        if (string.IsNullOrEmpty(att.ShowtimesJson) || att.ShowtimesJson == "null" || att.ShowtimesJson == "[]") 
            return;

        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        const string sql = @"
        INSERT INTO ""History"".""DailyShowSchedule""
        (""AttractionId"", ""Date"", ""Showtimes"")
        VALUES (@id, CURRENT_DATE, @showtimes)
        ON CONFLICT (""AttractionId"", ""Date"") DO UPDATE SET
            ""Showtimes"" = EXCLUDED.""Showtimes"";";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("id", att.AttractionId);
        cmd.Parameters.Add(new NpgsqlParameter("showtimes", NpgsqlDbType.Jsonb) { Value = att.ShowtimesJson });

        await cmd.ExecuteNonQueryAsync();
    }
}