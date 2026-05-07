using Npgsql;
using ParkPal.Common.API.Enums;
using ParkPal.Common.API.Models;
using ParkPal.Common.Data.Interfaces;
using ParkPal.Common.Models;

namespace ParkPal.Common.Data;

public class AlertRepository(string connectionString) : IAlertRepository
{
    public async Task<bool> UpsertAlertAsync(CreateAlertRequest request)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // ⭐️ The magic UPSERT using our new composite unique constraint!
        const string sql = @"
            INSERT INTO ""Alerts"".""WaitTimeAlert"" 
                (""AppUserId"", ""AttractionId"", ""AlertType"", ""TargetWaitTime"", ""IsActive"")
            VALUES 
                (@userId, @attraction, @type, @target, TRUE)
            ON CONFLICT (""AppUserId"", ""AttractionId"") 
            DO UPDATE SET 
                ""TargetWaitTime"" = @target, 
                ""AlertType"" = @type, 
                ""IsActive"" = TRUE;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("userId", request.AppUserId);
        cmd.Parameters.AddWithValue("attraction", request.AttractionId);
        cmd.Parameters.AddWithValue("type", (int)request.AlertType);
        cmd.Parameters.AddWithValue("target", request.TargetWaitTime);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
    
    public async Task<List<UserAlertDto>> GetUserAlertsAsync(string appUserId)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        var alerts = new List<UserAlertDto>();

        const string sql = @"
        SELECT 
            w.""AttractionId"",
            a.""Name"" AS AttractionName,
            d.""Name"" AS DestinationName,
            w.""AlertType"",
            w.""TargetWaitTime"",
            w.""IsActive""
        FROM ""Alerts"".""WaitTimeAlert"" w
        INNER JOIN ""Static"".""Attraction"" a ON w.""AttractionId"" = a.""AttractionId""
        INNER JOIN ""Static"".""Park"" p ON a.""ParkId"" = p.""ParkId""
        INNER JOIN ""Static"".""Destination"" d ON p.""DestinationId"" = d.""DestinationId""
        WHERE w.""AppUserId"" = @userId
        ORDER BY d.""Name"", a.""Name"";";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("userId", appUserId);

        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            alerts.Add(new UserAlertDto
            {
                AttractionId = reader.GetString(0),
                AttractionName = reader.GetString(1),
                DestinationName = reader.GetString(2),
                AlertType = (AlertType)reader.GetInt32(3),
                TargetWaitTime = reader.GetInt32(4),
                IsActive = reader.GetBoolean(5)
            });
        }

        return alerts;
    }
    
    public async Task<bool> DeleteAlertAsync(string appUserId, string attractionId)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // ⭐️ Only deletes if the User ID matches, keeping Spencer from deleting Abi's alerts!
        const string sql = @"
        DELETE FROM ""Alerts"".""WaitTimeAlert"" 
        WHERE ""AppUserId"" = @userId AND ""AttractionId"" = @attractionId;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("userId", appUserId);
        cmd.Parameters.AddWithValue("attractionId", attractionId);

        var rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<bool> ToggleAlertStatusAsync(string appUserId, string attractionId, bool isActive)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        const string sql = @"
        UPDATE ""Alerts"".""WaitTimeAlert"" 
        SET ""IsActive"" = @isActive 
        WHERE ""AppUserId"" = @userId AND ""AttractionId"" = @attractionId;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("userId", appUserId);
        cmd.Parameters.AddWithValue("attractionId", attractionId);
        cmd.Parameters.AddWithValue("isActive", isActive);

        var rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0;
    }
}