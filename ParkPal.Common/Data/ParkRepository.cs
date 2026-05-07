using System.Text.Json;
using Npgsql;
using ParkPal.Common.API.Models;
using ParkPal.Common.API.Models.Dtos;
using ParkPal.Common.Data.Interfaces;
using ParkPal.Common.Models;
using ParkPal.Common.Models.Enums;

namespace ParkPal.Common.Data;

public class ParkRepository(string connectionString, string cdnBaseUrl) : IParkRepository
{
    public async Task<List<Destination>> GetActiveDestinationsAsync()
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // ⭐️ UPDATE: INNER JOIN prevents empty Destinations, EXISTS prevents empty Parks!
        const string sql = @"
            SELECT d.""DestinationId"", 
                   d.""Name"" AS ""DestName"", 
                   d.""TimeZone"" AS ""Timezone"",
                   d.""Latitude"",
                   d.""Longitude"",
                   p.""ParkId"", 
                   p.""Name"" AS ""ParkName"",
                   p.""ImageUrl"", 
                   p.""ImageBlurHash"",
                   p.""Latitude"",
                   p.""Longitude""
            FROM ""Static"".""Destination"" d
            INNER JOIN ""Static"".""Park"" p ON d.""DestinationId"" = p.""DestinationId""
            WHERE EXISTS (
                SELECT 1 
                FROM ""Static"".""Attraction"" a 
                WHERE a.""ParkId"" = p.""ParkId"" AND a.""IsHidden"" = FALSE
            )
            ORDER BY d.""Name"", p.""Name"";";

        await using var cmd = new NpgsqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        var destDictionary = new Dictionary<string, Destination>();

        while (await reader.ReadAsync())
        {
            var destId = reader.GetString(0);
            var destName = reader.GetString(1);
            var destTimeZone = reader.GetString(2);
            var destLatitude = reader.GetDouble(3);
            var destLongitude = reader.GetDouble(4);
            
            if (!destDictionary.TryGetValue(destId, out var destination))
            {
                destination = new Destination(destId, destName)
                {
                    Timezone = destTimeZone,
                    Latitude = destLatitude,
                    Longitude = destLongitude
                };
                destDictionary.Add(destId, destination);
            }

            if (!reader.IsDBNull(5))
            {
                var parkId = reader.GetString(5);
                var parkName = reader.GetString(6);
                
                // Safely grab the image data
                var rawImageUrl = reader.IsDBNull(7) ? null : reader.GetString(7);
                var blurHash = reader.IsDBNull(8) ? null : reader.GetString(8);

                destination.Parks.Add(new Park(parkId, parkName)
                {
                    ImageUrl = rawImageUrl != null ? $"{cdnBaseUrl}{rawImageUrl}" : null,
                    ImageBlurHash = blurHash,
                    Latitude = reader.GetDouble(9),
                    Longitude = reader.GetDouble(10),
                });
            }
        }

        return destDictionary.Values.ToList();
    }
    
    public async Task<Destination?> GetDestinationWithParksAsync(string destinationId)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        Destination? destination = null;

        // ⭐️ Bulletproof logic applied here too!
        const string sql = @"
            SELECT d.""DestinationId"", d.""Name"" AS ""DestName"", 
                   p.""ParkId"", p.""Name"" AS ""ParkName"",
                   p.""ImageUrl"", p.""ImageBlurHash"", p.""Latitude"", p.""Longitude""
            FROM ""Static"".""Destination"" d
            INNER JOIN ""Static"".""Park"" p ON d.""DestinationId"" = p.""DestinationId""
            WHERE d.""DestinationId"" = @destId
              AND EXISTS (
                  SELECT 1 
                  FROM ""Static"".""Attraction"" a 
                  WHERE a.""ParkId"" = p.""ParkId"" AND a.""IsHidden"" = FALSE
              )
            ORDER BY p.""Name"";";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("destId", destinationId);
        
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            // Initialize the destination object on the first row
            if (destination == null)
            {
                destination = new Destination(reader.GetString(0), reader.GetString(1));
            }

            // If the LEFT JOIN found a park, add it!
            if (!reader.IsDBNull(2))
            {
                var parkId = reader.GetString(2);
                var parkName = reader.GetString(3);
                
                // Safely grab the image data
                var rawImageUrl = reader.IsDBNull(4) ? null : reader.GetString(4);
                var blurHash = reader.IsDBNull(5) ? null : reader.GetString(5);

                destination.Parks.Add(new Park(parkId, parkName)
                {
                    ImageUrl = rawImageUrl != null ? $"{cdnBaseUrl}{rawImageUrl}" : null,
                    ImageBlurHash = blurHash,
                    Latitude = reader.GetDouble(6),
                    Longitude = reader.GetDouble(7)
                });
            }
        }

        return destination;
    }
    
    public async Task<List<BaseAttractionDto>> GetParkAttractionsAsync(string parkId)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // ⭐️ A beautiful, lightweight query. No joins, no math. Just the catalog!
        const string sql = @"
        SELECT ""AttractionId"", ""Name"" 
        FROM ""Static"".""Attraction"" 
        WHERE ""ParkId"" = @parkId
        AND ""EntityType"" = 'ATTRACTION'
        ORDER BY ""Name"";";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("parkId", parkId);

        var results = new List<BaseAttractionDto>();
        await using var reader = await cmd.ExecuteReaderAsync();
    
        while (await reader.ReadAsync())
        {
            results.Add(new BaseAttractionDto(reader.GetString(0), reader.GetString(1)));
        }

        return results;
    }
    
    public async Task<Park?> GetParkDataAsync(string parkId)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // 1. Get the Park Name AND Images first
        // ⭐️ Switched to a reader so we can pull all 3 columns
        const string parkSql = @"
            SELECT ""Name"", ""ImageUrl"", ""ImageBlurHash"", ""Latitude"", ""Longitude""
            FROM ""Static"".""Park"" 
            WHERE ""ParkId"" = @parkId;";
            
        await using var parkCmd = new NpgsqlCommand(parkSql, conn);
        parkCmd.Parameters.AddWithValue("parkId", parkId);
        
        await using var parkReader = await parkCmd.ExecuteReaderAsync();
        
        if (!await parkReader.ReadAsync()) return null; // Park doesn't exist

        var parkName = parkReader.GetString(0);
        var rawParkImageUrl = parkReader.IsDBNull(1) ? null : parkReader.GetString(1);
        var parkBlurHash = parkReader.IsDBNull(2) ? null : parkReader.GetString(2);
        double? latitude = parkReader.IsDBNull(3) ? null : parkReader.GetDouble(3);
        double? longitude = parkReader.IsDBNull(4) ? null : parkReader.GetDouble(4);

        var park = new Park(parkId, parkName)
        {
            ImageUrl = rawParkImageUrl != null ? $"{cdnBaseUrl}{rawParkImageUrl}" : null,
            ImageBlurHash = parkBlurHash,
            Latitude = latitude,
            Longitude = longitude
        };
        
        // ⭐️ CRITICAL: Close the park reader before opening the ride reader!
        await parkReader.CloseAsync(); 

        return park;
    }
    
    public async Task<ParkLocationDto?> GetParkLocationForAttractionAsync(string attractionId)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // ⭐️ Hop straight from the Attraction to the Park to grab the coordinates!
        const string sql = @"
            SELECT p.""Latitude"", p.""Longitude""
            FROM ""Static"".""Park"" p
            INNER JOIN ""Static"".""Attraction"" a ON p.""ParkId"" = a.""ParkId""
            WHERE a.""AttractionId"" = @attractionId;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("attractionId", attractionId);
        
        await using var reader = await cmd.ExecuteReaderAsync();
    
        if (await reader.ReadAsync())
        {
            return new ParkLocationDto
            {
                // Safely handle parks that might not have GPS coordinates yet
                Latitude = reader.IsDBNull(0) ? null : reader.GetDouble(0),
                Longitude = reader.IsDBNull(1) ? null : reader.GetDouble(1)
            };
        }

        return null; // Attraction doesn't exist
    }

    public async Task<Park?> GetParkWithLiveAttractionsAsync(string parkId)
{
    await using var conn = new NpgsqlConnection(connectionString);
    await conn.OpenAsync();

    // 1. ⭐️ UPGRADED: Get the Park Name, Images, AND Destination Timezone!
    const string parkSql = @"
        SELECT 
            p.""Name"", 
            p.""ImageUrl"", 
            p.""ImageBlurHash"", 
            p.""Latitude"", 
            p.""Longitude"",
            d.""TimeZone"" -- ⭐️ NEW COLUMN (Index 5)
        FROM ""Static"".""Park"" p
        LEFT JOIN ""Static"".""Destination"" d ON p.""DestinationId"" = d.""DestinationId""
        WHERE p.""ParkId"" = @parkId;";
        
    await using var parkCmd = new NpgsqlCommand(parkSql, conn);
    parkCmd.Parameters.AddWithValue("parkId", parkId);
    
    await using var parkReader = await parkCmd.ExecuteReaderAsync();
    if (!await parkReader.ReadAsync()) return null; 

    var park = new Park(parkId, parkReader.GetString(0))
    {
        ImageUrl = parkReader.IsDBNull(1) ? null : $"{cdnBaseUrl}{parkReader.GetString(1)}",
        ImageBlurHash = parkReader.IsDBNull(2) ? null : parkReader.GetString(2),
        Latitude = parkReader.IsDBNull(3) ? null : parkReader.GetDouble(3),
        Longitude = parkReader.IsDBNull(4) ? null : parkReader.GetDouble(4),
        
        // ⭐️ Map the new Timezone string right here!
        Timezone = parkReader.IsDBNull(5) ? null : parkReader.GetString(5) 
    };
    
    await parkReader.CloseAsync();

    // 2. ⭐️ THE UPGRADED LIVE ATTRACTIONS QUERY
    const string rideSql = @"
        WITH ValidSubmissions AS (
            SELECT 
                s.""AttractionId"",
                s.""ReportedWaitTime"",
                s.""CreatedAt"",
                CASE 
                    WHEN p.""TrustScore"" >= 200 THEN 1.0  
                    WHEN p.""TrustScore"" >= 50 THEN 0.5   
                    ELSE 0.2                              
                END AS TrustWeight
            FROM ""CrowdSource"".""Submission"" s
            INNER JOIN ""Users"".""Profile"" p ON s.""AppUserId"" = p.""AppUserId""
            WHERE s.""ReportedWaitTime"" IS NOT NULL
              AND p.""TrustScore"" >= 0 
              AND s.""CreatedAt"" > NOW() - INTERVAL '45 minutes'
        ),
        CommunityStats AS (
            SELECT 
                ""AttractionId"",
                SUM(""ReportedWaitTime"" * TrustWeight) / NULLIF(SUM(TrustWeight), 0) AS CommunityWait,
                MAX(""CreatedAt"") AS LatestUpdate
            FROM ValidSubmissions
            GROUP BY ""AttractionId"" 
        )
        SELECT a.""AttractionId"", a.""Name"", a.""IsThrill"", 
               s.""WaitTime"", s.""Status"", s.""LastUpdated"",
               s.""SingleRiderWaitTime"", s.""LightningLaneReturnStart"", s.""LightningLanePrice"", s.""IsVirtualQueueOnly"",
               cs.CommunityWait, cs.LatestUpdate,
               a.""EntityType"", a.""Latitude"", a.""Longitude"", s.""Showtimes"" -- ⭐️ NEW COLUMNS (12, 13, 14, 15)
        FROM ""Static"".""Attraction"" a
        LEFT JOIN ""Live"".""AttractionState"" s ON a.""AttractionId"" = s.""AttractionId""
        LEFT JOIN CommunityStats cs ON a.""AttractionId"" = cs.""AttractionId""
        WHERE a.""ParkId"" = @parkId 
          AND a.""IsHidden"" = FALSE
        ORDER BY a.""Name"";";

    await using var rideCmd = new NpgsqlCommand(rideSql, conn);
    rideCmd.Parameters.AddWithValue("parkId", parkId);
    await using var reader = await rideCmd.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
        // Safely parse the EntityType enum, defaulting to ATTRACTION if it goes wrong
        var entityTypeString = reader.IsDBNull(12) ? "ATTRACTION" : reader.GetString(12);
        if (!Enum.TryParse<EntityType>(entityTypeString, out var parsedEntityType))
        {
            parsedEntityType = EntityType.ATTRACTION;
        }
        
        var rawShowtimesJson = reader.IsDBNull(15) ? null : reader.GetString(15);
    
        // 2. Unpack it into a real C# List!
        var parsedShowtimes = rawShowtimesJson != null 
            ? JsonSerializer.Deserialize<List<ShowtimeDto>>(rawShowtimesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) 
            : null;

        var attraction = new AttractionDto(
            reader.GetString(0), 
            reader.GetString(1), 
            parsedEntityType, // ⭐️ Passing the new Enum here!
            reader.IsDBNull(4) ? ParkPalAttractionStatus.Closed : (ParkPalAttractionStatus)reader.GetInt32(4)
        )
        {
            WaitTime = reader.IsDBNull(3) ? null : reader.GetInt32(3), // WaitTime moved here!
            Thrill = reader.GetBoolean(2),
            LastUpdated = reader.IsDBNull(5) ? null : reader.GetFieldValue<DateTimeOffset>(5),
            SingleRiderWaitTime = reader.IsDBNull(6) ? null : reader.GetInt32(6),
            LightningLaneReturnStart = reader.IsDBNull(7) ? null : reader.GetFieldValue<DateTimeOffset>(7),
            LightningLanePrice = reader.IsDBNull(8) ? null : reader.GetDouble(8),
            IsVirtualQueueOnly = !reader.IsDBNull(9) && reader.GetBoolean(9),
            
            CommunityWaitTime = reader.IsDBNull(10) ? null : (int)Math.Round(reader.GetDouble(10)),
            LastCommunityUpdate = reader.IsDBNull(11) ? null : reader.GetFieldValue<DateTimeOffset>(11),
            
            // ⭐️ Mapping the new fields safely!
            Latitude = reader.IsDBNull(13) ? null : reader.GetDouble(13),
            Longitude = reader.IsDBNull(14) ? null : reader.GetDouble(14),
            ShowtimesJson = rawShowtimesJson,
            Showtimes = parsedShowtimes,
        };

        park.Attractions.Add(attraction);
    }

    return park;
}
    
    public async Task<AttractionChartResponse> GetAttractionChartDataAsync(string attractionId)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        var chartData = new AttractionChartResponse();

        const string sql = @"
    WITH tz_info AS (
        -- 1. Grab the exact timezone for this specific ride!
        SELECT d.""TimeZone"" 
        FROM ""Static"".""Attraction"" a
        JOIN ""Static"".""Park"" p ON a.""ParkId"" = p.""ParkId""
        JOIN ""Static"".""Destination"" d ON p.""DestinationId"" = d.""DestinationId""
        WHERE a.""AttractionId"" = @id
    ),
    time_buckets AS (
        -- 2. ⭐️ THE UPGRADE: Generate 00:00 to 23:30 buckets strictly in the PARK'S local time
        SELECT generate_series(
            ((CURRENT_TIMESTAMP AT TIME ZONE (SELECT ""TimeZone"" FROM tz_info))::date + interval '0 hours'), 
            ((CURRENT_TIMESTAMP AT TIME ZONE (SELECT ""TimeZone"" FROM tz_info))::date + interval '23 hours 30 minutes'), 
            interval '30 minutes'
        ) AS bucket_time
    )
    SELECT 
        -- ⭐️ THE FIX: Convert local bucket to True UTC (and fixed the quadruple quotes!)
        (tb.bucket_time AT TIME ZONE (SELECT ""TimeZone"" FROM tz_info)) AS bucket_utc,
        AVG(h.""WaitTime"")::int AS AvgWait,
        (SELECT ""TimeZone"" FROM tz_info) AS TimeZoneStr 
    FROM time_buckets tb
    LEFT JOIN ""History"".""Attraction"" h 
        ON h.""AttractionId"" = @id
        -- 3. Ensure the historical record happened on the same DAY OF THE WEEK in the park's local time
        AND EXTRACT(DOW FROM (h.""StartTime"" AT TIME ZONE (SELECT ""TimeZone"" FROM tz_info))) = EXTRACT(DOW FROM (CURRENT_TIMESTAMP AT TIME ZONE (SELECT ""TimeZone"" FROM tz_info)))
        -- 4. Ensure the historical record falls exactly into this 30 min bucket in the park's local time
        AND (h.""StartTime"" AT TIME ZONE (SELECT ""TimeZone"" FROM tz_info))::time >= tb.bucket_time::time 
        AND (h.""StartTime"" AT TIME ZONE (SELECT ""TimeZone"" FROM tz_info))::time < (tb.bucket_time + interval '30 minutes')::time
    GROUP BY tb.bucket_time
    ORDER BY tb.bucket_time;

    -- ⭐️ QUERY 2: Get Today's Data
    WITH tz_info AS (
        SELECT d.""TimeZone"" 
        FROM ""Static"".""Attraction"" a
        JOIN ""Static"".""Park"" p ON a.""ParkId"" = p.""ParkId""
        JOIN ""Static"".""Destination"" d ON p.""DestinationId"" = d.""DestinationId""
        WHERE a.""AttractionId"" = @id
    )
    SELECT ""StartTime"", ""WaitTime""
    FROM ""History"".""Attraction""
    WHERE ""AttractionId"" = @id
      -- 5. Only grab records where the localized start time happened TODAY in the park's timezone!
      AND (""StartTime"" AT TIME ZONE (SELECT ""TimeZone"" FROM tz_info))::date = (CURRENT_TIMESTAMP AT TIME ZONE (SELECT ""TimeZone"" FROM tz_info))::date
    ORDER BY ""StartTime"";";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("id", attractionId);
        
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var bucketTime = reader.GetFieldValue<DateTime>(0);
            var utcDate = new DateTimeOffset(bucketTime, TimeSpan.Zero); 
            
            // ⭐️ Grab the TimeZone string from column 2!
            if (!reader.IsDBNull(2) && string.IsNullOrEmpty(chartData.TimeZone) || chartData.TimeZone == "UTC")
            {
                chartData.TimeZone = reader.GetString(2);
            }
            
            if (!reader.IsDBNull(1))
            {
                var avgWait = reader.GetInt32(1);
                chartData.HistoricalData.Add(new WaitTimeTrendDto { Date = utcDate, WaitTime = avgWait });
            }
        }

        if (await reader.NextResultAsync()) 
        {
            while (await reader.ReadAsync())
            {
                var startTime = reader.GetFieldValue<DateTimeOffset>(0);
                if (!reader.IsDBNull(1))
                {
                    var waitTime = reader.GetInt32(1);
                
                    // ⭐️ Changed to >= 0 so we don't accidentally ignore true "Walk-On" (0 min) waits!
                    if (waitTime >= 0)
                    {
                        chartData.TodayData.Add(new WaitTimeTrendDto { Date = startTime, WaitTime = waitTime });
                    }
                }
            }
        }

        return chartData;
    }
    
    public async Task<CatalogResponseDto> GetWidgetEntityCatalogByTypeAsync(string entityType)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // ⭐️ 2. Use the @EntityType variable
        const string sql = @"
        SELECT d.""Name"" AS ""DestName"", 
               p.""ParkId"", p.""Name"" AS ""ParkName"", 
               a.""AttractionId"", a.""Name"" AS ""AttractionName""
        FROM ""Static"".""Attraction"" a
        INNER JOIN ""Static"".""Park"" p ON a.""ParkId"" = p.""ParkId""
        INNER JOIN ""Static"".""Destination"" d ON p.""DestinationId"" = d.""DestinationId""
        WHERE a.""IsHidden"" = FALSE
        AND a.""EntityType"" = @EntityType 
        ORDER BY d.""Name"", p.""Name"", a.""Name"";";

        await using var cmd = new NpgsqlCommand(sql, conn);
    
        // ⭐️ 3. Bind the parameter safely (forcing uppercase just in case!)
        cmd.Parameters.AddWithValue("EntityType", entityType.ToUpper());

        await using var reader = await cmd.ExecuteReaderAsync();

        var destinationsMap = new Dictionary<string, DestinationCatalogDto>();

        while (await reader.ReadAsync())
        {
            var destName = reader.GetString(0);
            var parkId = reader.GetString(1);
            var parkName = reader.GetString(2);
            var attrId = reader.GetString(3);
            var attrName = reader.GetString(4);

            if (!destinationsMap.TryGetValue(destName, out var destination))
            {
                destination = new DestinationCatalogDto { Name = destName };
                destinationsMap.Add(destName, destination);
            }

            var park = destination.Parks.FirstOrDefault(p => p.ParkId == parkId);
            if (park == null)
            {
                park = new ParkCatalogDto { ParkId = parkId, Name = parkName };
                destination.Parks.Add(park);
            }

            park.Attractions.Add(new AttractionCatalogDto 
            { 
                AttractionId = attrId, 
                Name = attrName 
            });
        }

        return new CatalogResponseDto 
        { 
            Destinations = destinationsMap.Values.ToList() 
        };
    }
    
    public async Task<LiveAttractionStatusDto?> GetLiveAttractionStatusAsync(string attractionId)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // ⭐️ A highly targeted query for a single attraction
        const string sql = @"
            SELECT a.""AttractionId"", a.""Name"", s.""WaitTime"", s.""Status""
            FROM ""Static"".""Attraction"" a
            LEFT JOIN ""Live"".""AttractionState"" s ON a.""AttractionId"" = s.""AttractionId""
            WHERE a.""AttractionId"" = @id;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("id", attractionId);
        
        await using var reader = await cmd.ExecuteReaderAsync();
    
        if (await reader.ReadAsync())
        {
            return new LiveAttractionStatusDto
            {
                AttractionId = reader.GetString(0),
                Name = reader.GetString(1),
                WaitTime = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                // If there's no live state row yet, default to Closed (2)
                Status = reader.IsDBNull(3) ? 2 : reader.GetInt32(3) 
            };
        }

        return null; // Attraction doesn't exist in the database
    }
    
    public async Task<UpcomingShowsDto?> GetUpcomingShowtimesAsync(string attractionId)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        const string sql = @"
        SELECT a.""AttractionId"", a.""Name"",
               d.""TimeZone"" AS ""ParkTimezone"",
               (st.show->>'StartTime')::timestamptz AS ""StartTime"",
               s.""Status"", 
               s.""Showtimes"" AS ""RawShowtimes"" -- ⭐️ Grab the raw JSON to check for empty/null
        FROM ""Static"".""Attraction"" a
        INNER JOIN ""Static"".""Park"" p ON a.""ParkId"" = p.""ParkId""
        INNER JOIN ""Static"".""Destination"" d ON p.""DestinationId"" = d.""DestinationId""
        LEFT JOIN ""Live"".""AttractionState"" s ON a.""AttractionId"" = s.""AttractionId""
        
        LEFT JOIN LATERAL jsonb_array_elements(
               CASE WHEN jsonb_typeof(s.""Showtimes"") = 'array' 
                    THEN s.""Showtimes"" 
                    ELSE '[]'::jsonb 
               END
        ) AS st(show) 
               ON (st.show->>'StartTime') IS NOT NULL 
               AND (st.show->>'StartTime')::timestamptz > @now
               
        WHERE a.""AttractionId"" = @id
        ORDER BY ""StartTime"" ASC;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("id", attractionId);
        cmd.Parameters.AddWithValue("now", DateTime.UtcNow);

        await using var reader = await cmd.ExecuteReaderAsync();

        UpcomingShowsDto? result = null;

        while (await reader.ReadAsync())
        {
            if (result == null)
            {
                // ⭐️ Get status and raw showtimes for the continuous check
                int? status = reader.IsDBNull(4) ? null : reader.GetInt32(4);
                string rawShowtimes = reader.IsDBNull(5) ? null : reader.GetString(5);

                result = new UpcomingShowsDto
                {
                    AttractionId = reader.GetString(0),
                    Name = reader.GetString(1),
                    ParkTimezone = reader.GetString(2),
                    // ⭐️ THE LOGIC: Status 0 (Operating) AND (No JSON or empty array)
                    IsContinuous = status == 0 && (string.IsNullOrEmpty(rawShowtimes) || rawShowtimes == "[]")
                };
            }

            if (!reader.IsDBNull(3))
            {
                result.UpcomingShowtimes.Add(reader.GetFieldValue<DateTime>(3));
                
                // 🛡️ Extra Safety: If we actually have upcoming showtimes, 
                // it definitely isn't continuous!
                result.IsContinuous = false;
            }
        }

        return result; 
    }

    public async Task<List<RestaurantDto>> GetRestaurantsForParkAsync(string parkId)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();
    
        // ⭐️ THE FIX: Cleaned up the query and added the EntityType filter!
        // (Adjust the EntityType value to whatever your DB uses for Restaurants, e.g., 'Restaurant' or an integer like 2)
        const string sql = @"
            SELECT a.""Name"",
                   a.""AttractionId""
            FROM ""Static"".""Attraction"" a
            WHERE a.""IsHidden"" = FALSE
            AND a.""ParkId"" = @parkId
            AND a.""EntityType"" = 'RESTAURANT' 
            ORDER BY a.""Name"";";
    
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("parkId", parkId);
        await using var reader = await cmd.ExecuteReaderAsync();
    
        var restaurants = new List<RestaurantDto>();
    
        while (await reader.ReadAsync())
        {
            restaurants.Add(new RestaurantDto(reader.GetString(1), reader.GetString(0)));
        }
    
        return restaurants;
    }
    
    public async Task<List<PlannerShowDto>> GetShowsForParkAsync(string parkId, DateTime requestedDate)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // ⭐️ THE UPGRADE: Joined Park and Destination to grab the TimeZone!
        const string sql = @"
            SELECT 
                a.""AttractionId"",
                a.""Name"",
                COALESCE(
                    (SELECT s.""Showtimes"" FROM ""Live"".""AttractionState"" s WHERE s.""AttractionId"" = a.""AttractionId"" AND @reqDate = CURRENT_DATE),
                    (SELECT h.""Showtimes"" FROM ""History"".""DailyShowSchedule"" h WHERE h.""AttractionId"" = a.""AttractionId"" AND EXTRACT(DOW FROM h.""Date"") = @dow ORDER BY h.""Date"" DESC LIMIT 1),
                    (SELECT s.""Showtimes"" FROM ""Live"".""AttractionState"" s WHERE s.""AttractionId"" = a.""AttractionId""),
                    '[]'::jsonb
                ) AS ProjectedTimes,
                d.""TimeZone"" -- ⭐️ Grabbed the timezone string!
            FROM ""Static"".""Attraction"" a
            INNER JOIN ""Static"".""Park"" p ON a.""ParkId"" = p.""ParkId""
            INNER JOIN ""Static"".""Destination"" d ON p.""DestinationId"" = d.""DestinationId""
            WHERE a.""ParkId"" = @parkId 
            AND a.""EntityType"" = 'SHOW' 
            AND a.""IsHidden"" = FALSE
            ORDER BY a.""Name"";";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("parkId", parkId);
        cmd.Parameters.AddWithValue("reqDate", requestedDate.Date);
        cmd.Parameters.AddWithValue("dow", (int)requestedDate.DayOfWeek);

        await using var reader = await cmd.ExecuteReaderAsync();
        var shows = new List<PlannerShowDto>();
        
        // A variable to hold our TimeZone rules once we read the first row
        TimeZoneInfo? parkTimeZone = null;

        while (await reader.ReadAsync())
        {
            var showId = reader.GetString(0);
            var name = reader.GetString(1);
            var showtimesJson = reader.GetString(2);
            var timeZoneString = reader.IsDBNull(3) ? "UTC" : reader.GetString(3);

            // ⭐️ If we haven't loaded the timezone yet, do it now!
            if (parkTimeZone == null)
            {
                try 
                {
                    // If you are hosting on Windows and this crashes with IANA strings like "Europe/Paris", 
                    // install the "TimeZoneConverter" NuGet package and change this line to: 
                    // parkTimeZone = TZConvert.GetTimeZoneInfo(timeZoneString);
                    parkTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneString);
                }
                catch (TimeZoneNotFoundException)
                {
                    Console.WriteLine($"Warning: Could not find timezone '{timeZoneString}'. Falling back to UTC.");
                    parkTimeZone = TimeZoneInfo.Utc;
                }
            }

            var times = new List<string>();

            if (!string.IsNullOrWhiteSpace(showtimesJson) && showtimesJson != "[]")
            {
                try 
                {
                    using var doc = System.Text.Json.JsonDocument.Parse(showtimesJson);
                    foreach (var element in doc.RootElement.EnumerateArray())
                    {
                        System.Text.Json.JsonProperty? targetProp = null;
                        foreach (var prop in element.EnumerateObject())
                        {
                            if (prop.Name.Equals("startTime", StringComparison.OrdinalIgnoreCase))
                            {
                                targetProp = prop;
                                break;
                            }
                        }

                        if (targetProp.HasValue)
                        {
                            if (targetProp.Value.Value.TryGetDateTime(out var dt))
                            {
                                // ⭐️ THE MAGIC: Force it to UTC, then convert it to the Park's timezone!
                                var utcTime = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                                var localParkTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, parkTimeZone);
                                
                                times.Add(localParkTime.ToString("HH:mm"));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing showtimes for {name}: {ex.Message}");
                }
            }

            shows.Add(new PlannerShowDto
            {
                Id = showId,
                Name = name,
                Showtimes = times
            });
        }

        return shows;
    }

    public async Task<List<BaseAttractionDto>> GetAttractionsWithLocationsForPark(string parkId)
    {
        try
        {
            await using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();

            // ⭐️ A highly targeted query for a single attraction
            const string sql = @"
            SELECT a.""AttractionId"", a.""Name"",  a.""Latitude"", a.""Longitude""
            FROM ""Static"".""Attraction"" a
            LEFT JOIN ""Static"".""Park"" p ON a.""ParkId"" = p.""ParkId""
            WHERE a.""ParkId"" = @id;";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", parkId);

            var attractions = new List<BaseAttractionDto>();

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                attractions.Add(new BaseAttractionDto(reader.GetString(0), reader.GetString(1))
                {
                    Latitude = reader.IsDBNull(2) ? null : reader.GetDouble(2),
                    Longitude = reader.IsDBNull(3) ? null : reader.GetDouble(3),
                });
            }

            return attractions;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return [];
    }
    
    public async Task<List<string>> GetShowtimesAsync(string attractionId, DateTime requestedDate)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // ⭐️ The exact same fallback magic, but laser-focused on one show!
        const string sql = @"
            SELECT 
                COALESCE(
                    (SELECT s.""Showtimes"" FROM ""Live"".""AttractionState"" s WHERE s.""AttractionId"" = a.""AttractionId"" AND @reqDate = CURRENT_DATE),
                    (SELECT h.""Showtimes"" FROM ""History"".""DailyShowSchedule"" h WHERE h.""AttractionId"" = a.""AttractionId"" AND EXTRACT(DOW FROM h.""Date"") = @dow ORDER BY h.""Date"" DESC LIMIT 1),
                    (SELECT s.""Showtimes"" FROM ""Live"".""AttractionState"" s WHERE s.""AttractionId"" = a.""AttractionId""),
                    '[]'::jsonb
                ) AS ProjectedTimes,
                d.""TimeZone"" 
            FROM ""Static"".""Attraction"" a
            INNER JOIN ""Static"".""Park"" p ON a.""ParkId"" = p.""ParkId""
            INNER JOIN ""Static"".""Destination"" d ON p.""DestinationId"" = d.""DestinationId""
            WHERE a.""AttractionId"" = @attractionId;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("attractionId", attractionId);
        cmd.Parameters.AddWithValue("reqDate", requestedDate.Date);
        cmd.Parameters.AddWithValue("dow", (int)requestedDate.DayOfWeek);

        var times = new List<string>();
        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            var showtimesJson = reader.GetString(0);
            var timeZoneString = reader.IsDBNull(1) ? "UTC" : reader.GetString(1);

            if (!string.IsNullOrWhiteSpace(showtimesJson) && showtimesJson != "[]")
            {
                // ⭐️ Reusing your timezone logic!
                TimeZoneInfo parkTimeZone;
                try 
                {
                    parkTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneString);
                }
                catch (TimeZoneNotFoundException)
                {
                    parkTimeZone = TimeZoneInfo.Utc;
                }

                try 
                {
                    using var doc = System.Text.Json.JsonDocument.Parse(showtimesJson);
                    foreach (var element in doc.RootElement.EnumerateArray())
                    {
                        System.Text.Json.JsonProperty? targetProp = null;
                        foreach (var prop in element.EnumerateObject())
                        {
                            if (prop.Name.Equals("startTime", StringComparison.OrdinalIgnoreCase))
                            {
                                targetProp = prop;
                                break;
                            }
                        }

                        if (targetProp.HasValue && targetProp.Value.Value.TryGetDateTime(out var dt))
                        {
                            // Parse UTC and convert to the Park's local time zone
                            var utcTime = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                            var localParkTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, parkTimeZone);
                            
                            times.Add(localParkTime.ToString("HH:mm"));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing showtimes for {attractionId}: {ex.Message}");
                }
            }
        }

        return times;
    }
}