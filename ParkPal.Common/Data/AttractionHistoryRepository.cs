using Microsoft.Extensions.Configuration;
using Npgsql;
using ParkPal.Common.API.Models.Dtos;
using ParkPal.Common.Data.Interfaces;

namespace ParkPal.Common.Data;

public class AttractionHistoryRepository(string connectionString) : IAttractionHistoryRepository
{
    public async Task<List<HistoricalWaitTimeBucketDto>> GetAveragesForDayAsync(List<string> attractionIds, DayOfWeek dayOfWeek)
{
    var results = new List<HistoricalWaitTimeBucketDto>();
    if (!attractionIds.Any()) return results;

    await using var conn = new NpgsqlConnection(connectionString);
    await conn.OpenAsync();

    const string sql = @"
        WITH tz_info AS (
            -- ⭐️ 1. Grab the timezone AND the proper Name from your Static table
            SELECT a.""AttractionId"", a.""Name"", d.""TimeZone"" 
            FROM ""Static"".""Attraction"" a
            JOIN ""Static"".""Park"" p ON a.""ParkId"" = p.""ParkId""
            JOIN ""Static"".""Destination"" d ON p.""DestinationId"" = d.""DestinationId""
            WHERE a.""AttractionId"" = ANY(@ids::varchar[])
        ),
        time_buckets AS (
            -- 2. Generate 00:00 to 23:30 buckets
            SELECT generate_series(
                '2000-01-01 00:00:00'::timestamp, 
                '2000-01-01 23:30:00'::timestamp, 
                interval '30 minutes'
            )::time AS bucket_time
        )
        SELECT 
            h.""AttractionId"",
            tb.bucket_time,
            AVG(h.""WaitTime"")::int AS AvgWait,
            -- ⭐️ 3. Use the name from our tz_info CTE instead of the raw JSON
            MAX(tz.""Name"") AS AttractionName 
        FROM time_buckets tb
        JOIN ""History"".""Attraction"" h ON h.""AttractionId"" = ANY(@ids::varchar[])
        JOIN tz_info tz ON tz.""AttractionId"" = h.""AttractionId""
        
        AND EXTRACT(DOW FROM (h.""StartTime"" AT TIME ZONE tz.""TimeZone"")) = @dow::int
        AND (h.""StartTime"" AT TIME ZONE tz.""TimeZone"")::time >= tb.bucket_time
        AND (h.""StartTime"" AT TIME ZONE tz.""TimeZone"")::time < (tb.bucket_time + interval '30 minutes')
        
        AND h.""WaitTime"" > 0 
        GROUP BY h.""AttractionId"", tb.bucket_time
        ORDER BY h.""AttractionId"", tb.bucket_time;";

    await using var cmd = new NpgsqlCommand(sql, conn);
    // Explicitly cast the IDs to lowercase to match your DB and use varchar[]
    cmd.Parameters.AddWithValue("ids", attractionIds.Select(x => x.ToLower()).ToArray());
    cmd.Parameters.AddWithValue("dow", (int)dayOfWeek); 

    try
    {
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new HistoricalWaitTimeBucketDto()
            {
                AttractionId = reader.GetFieldValue<string>(0),
                BucketTime = reader.GetFieldValue<TimeSpan>(1),
                AverageWaitTime = reader.GetInt32(2),
                // ⭐️ Safely handle the string (though with Static table, it shouldn't be null!)
                AttractionName = reader.IsDBNull(3) ? "Unknown Attraction" : reader.GetString(3)
            });
        }
    }
    catch (Exception e)
    {
        Console.WriteLine($"[DB ERROR] GetAveragesForDayAsync: {e.Message}");
    }

    return results;
}
}