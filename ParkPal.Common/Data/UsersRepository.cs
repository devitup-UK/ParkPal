using Npgsql;
using ParkPal.Common.API.Models.Dtos;
using ParkPal.Common.Data.Interfaces;

namespace ParkPal.Common.Data;

public class UsersRepository(string connectionString) : IUsersRepository
{
    public async Task RegisterDeviceHandshakeAsync(UserRegistrationDto registration)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        const string sql = @"
        -- ⭐️ FIX 1: Lazy Profile Creation (Solves the 500 Error!)
        -- If they don't exist, create them with default 0 TrustScore. If they do, ignore.
        INSERT INTO ""Users"".""Profile"" (""AppUserId"") 
        VALUES (@userId) 
        ON CONFLICT (""AppUserId"") DO NOTHING;

        -- ⭐️ FIX 2: Your existing APNS Device handshake
        INSERT INTO ""Users"".""Device"" (""DeviceToken"", ""AppUserId"", ""LastActiveAt"") 
        VALUES (@token, @userId, now()) 
        ON CONFLICT (""DeviceToken"") DO UPDATE SET 
            ""AppUserId"" = EXCLUDED.""AppUserId"",
            ""LastActiveAt"" = now();";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("token", registration.DeviceToken);
        cmd.Parameters.AddWithValue("userId", registration.AppUserId);
    
        await cmd.ExecuteNonQueryAsync();
    }
    
    public async Task IncreaseUserTrustScoreAsync(string userId)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // ⭐️ ATOMIC UPDATE: We increment both values in a single shot.
        // We cap the score at 1000 so it doesn't grow to infinity.
        const string sql = @"
        UPDATE ""Users"".""Profile"" 
        SET ""TrustScore"" = LEAST(""TrustScore"" + 5, 1000), 
            ""TotalSubmissions"" = ""TotalSubmissions"" + 1
        WHERE ""AppUserId"" = @userId;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("userId", userId);
    
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DecreaseUserTrustScoreAsync(string userId, int penalty)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // ⭐️ ATOMIC UPDATE: Subtract the penalty.
        // We floor the score at -100 so they stay shadowbanned but not 'mathematically dead'.
        const string sql = @"
        UPDATE ""Users"".""Profile"" 
        SET ""TrustScore"" = GREATEST(""TrustScore"" - @penalty, -100), 
            ""TotalSubmissions"" = ""TotalSubmissions"" + 1
        WHERE ""AppUserId"" = @userId;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("userId", userId);
        cmd.Parameters.AddWithValue("penalty", penalty);
    
        await cmd.ExecuteNonQueryAsync();
    }
    
    public async Task<UserProfileDto?> GetProfileAsync(string userId)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        const string sql = @"SELECT ""TotalSubmissions"", ""TrustScore"", ""FirstSeenAt"" 
                        FROM ""Users"".""Profile"" 
                        WHERE ""AppUserId"" = @userId;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("userId", userId);
    
        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new UserProfileDto() {
                TotalSubmissions = reader.GetInt32(0),
                TrustScore = reader.GetInt32(1),
                FirstSeenAt = reader.GetDateTime(2)
            };
        }
        return null;
    }
}