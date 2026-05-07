using Npgsql;
using ParkPal.Common.Data.Interfaces;

namespace ParkPal.Common.Data;

public class DeviceRepository(string connectionString) : IDeviceRepository
{
    public async Task UpsertDeviceAsync(string appUserId, string deviceToken)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // ⭐️ The 'ON CONFLICT' magic is perfect here. 
        // We use DeviceToken as the conflict target because it is the Primary Key.
        const string sql = @"
            INSERT INTO ""Users"".""Device"" (""DeviceToken"", ""AppUserId"", ""LastActiveAt"")
            VALUES (@token, @userId, now())
            ON CONFLICT (""DeviceToken"") 
            DO UPDATE SET 
                ""AppUserId"" = @userId, 
                ""LastActiveAt"" = now();";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("token", deviceToken);
        cmd.Parameters.AddWithValue("userId", appUserId);

        await cmd.ExecuteNonQueryAsync();
    }
}