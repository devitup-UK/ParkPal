using Npgsql;
using ParkPal.Common.API.Models.Dtos;
using ParkPal.Common.Data.Interfaces;

namespace ParkPal.Common.Data;

public class CrowdSourceRepository(string connectionString) : ICrowdSourceRepository
{
    public async Task SubmitAttractionStateAsync(string userId, AttractionSubmissionDto submission)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        const string sql = @"
            INSERT INTO ""CrowdSource"".""Submission"" 
            (""AppUserId"", ""AttractionId"", ""ReportedStatus"", ""ReportedWaitTime"", ""Latitude"", ""Longitude"")
            VALUES (@userId, @attractionId, @status, @wait, @lat, @long);";

        await using var cmd = new NpgsqlCommand(sql, conn);
        
        cmd.Parameters.AddWithValue("userId", userId);
        cmd.Parameters.AddWithValue("attractionId", submission.AttractionId);
        cmd.Parameters.AddWithValue("status", submission.ReportedStatus);
        
        // ⭐️ If the ride is Down/Closed, wait time should be NULL
        cmd.Parameters.AddWithValue("wait", (object?)submission.ReportedWaitTime ?? DBNull.Value);
        
        // ⭐️ Storing location for future geofence validation!
        cmd.Parameters.AddWithValue("lat", (object?)submission.Latitude ?? DBNull.Value);
        cmd.Parameters.AddWithValue("long", (object?)submission.Longitude ?? DBNull.Value);

        await cmd.ExecuteNonQueryAsync();
        
        // 💡 Future home of: await IncrementUserTrustScoreAsync(userId);
    }
}