using Npgsql;
using ParkPal.Common.API.Models.Dtos;

namespace ParkPal.Common.Data.Interfaces;

public class LiveActivityRepository(string connectionString) : ILiveActivityRepository
{
    public async Task RegisterMonitorAsync(string appUserId, RegisterLiveActivityRequest request)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // ⭐️ The Senior Dev Upsert: Insert it, or update the token if they are already monitoring it!
        const string sql = @"
            INSERT INTO ""Alerts"".""LiveActivityMonitor"" 
            (""AppUserId"", ""AttractionId"", ""PushToken"", ""ExpiresAt"") 
            VALUES (@userId, @attractionId, @pushToken, @expiresAt)
            ON CONFLICT (""AppUserId"", ""AttractionId"") 
            DO UPDATE SET 
                ""PushToken"" = EXCLUDED.""PushToken"",
                ""ExpiresAt"" = EXCLUDED.""ExpiresAt"";";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("userId", appUserId);
        cmd.Parameters.AddWithValue("attractionId", request.AttractionId);
        cmd.Parameters.AddWithValue("pushToken", request.PushToken);
        
        // Live Activities generally expire after 8 hours in iOS
        cmd.Parameters.AddWithValue("expiresAt", DateTime.UtcNow.AddHours(8));

        await cmd.ExecuteNonQueryAsync();
    }
    
    public async Task RemoveMonitorAsync(string appUserId, string attractionId)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        const string sql = @"
            DELETE FROM ""Alerts"".""LiveActivityMonitor"" 
            WHERE ""AppUserId"" = @userId AND ""AttractionId"" = @attractionId;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("userId", appUserId);
        cmd.Parameters.AddWithValue("attractionId", attractionId);

        await cmd.ExecuteNonQueryAsync();
    }
}