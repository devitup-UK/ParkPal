using Microsoft.Extensions.Configuration;
using Npgsql;
using ParkPal.Common.API.Models.Dtos;
using ParkPal.Common.Helpers;

namespace ParkPal.Common.Data.Interfaces;

public class ItineraryRepository(string connectionString) : IItineraryRepository
{

    public async Task SavePlanAsync(string appUserId, SavedPlanDto plan)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // 1. Start a transaction so if an item fails, the whole save rolls back safely
        await using var tx = await conn.BeginTransactionAsync();

        try
        {
            // 2. Insert the Parent Itinerary
            const string insertPlanSql = @"
                INSERT INTO ""Users"".""Itinerary"" 
                (""Id"", ""AppUserId"", ""Title"", ""TripDate"", ""DestinationName"", ""ParkName"", ""ArrivalTime"", ""DepartureTime"", ""ShareCode"") 
                VALUES (@id, @userId, @title, @date, @dest, @park, @arrivalTime, @departureTime, @shareCode);";

            await using var planCmd = new NpgsqlCommand(insertPlanSql, conn, tx);
            planCmd.Parameters.AddWithValue("id", plan.Id);
            planCmd.Parameters.AddWithValue("userId", appUserId);
            planCmd.Parameters.AddWithValue("title", plan.Title);
            planCmd.Parameters.AddWithValue("date", plan.TripDate);
            planCmd.Parameters.AddWithValue("dest", plan.DestinationName);
            planCmd.Parameters.AddWithValue("park", plan.ParkName);
            planCmd.Parameters.AddWithValue("arrivalTime", (object)plan.ArrivalTime ?? DBNull.Value);
            planCmd.Parameters.AddWithValue("departureTime", (object)plan.DepartureTime ?? DBNull.Value);
            planCmd.Parameters.AddWithValue("shareCode", DataHelper.GenerateShareCode());
            
            await planCmd.ExecuteNonQueryAsync();

            // 3. Insert all the schedule items
            const string insertItemSql = @"
                INSERT INTO ""Users"".""ItineraryItem"" 
                (""ItineraryId"", ""ScheduledTime"", ""CustomTitle"", ""CustomSubtitle"", ""ProjectedWaitTime"", ""IconName"", ""IconColour"", ""AttractionId"") 
                VALUES (@itineraryId, @time, @title, @subtitle, @wait, @iconName, @iconColour, @attractionId);";

            foreach (var item in plan.Items)
            {
                await using var itemCmd = new NpgsqlCommand(insertItemSql, conn, tx);
                itemCmd.Parameters.AddWithValue("itineraryId", plan.Id);
                // Convert "09:00 AM" string back to a TimeSpan for Postgres
                var scheduledTime = DateTime.Parse(item.Time).TimeOfDay;
                itemCmd.Parameters.AddWithValue("time", scheduledTime);
                itemCmd.Parameters.AddWithValue("title", item.Title);
                itemCmd.Parameters.AddWithValue("subtitle", item.Subtitle);
                
                // ⭐️ Safely extract the wait time using TryParse!
                var digitString = string.Join("", item.Subtitle.Where(char.IsDigit));
                int? projectedWait = null;
                
                if (!string.IsNullOrEmpty(digitString) && int.TryParse(digitString, out var parsedWait))
                {
                    projectedWait = parsedWait;
                }

                itemCmd.Parameters.AddWithValue("wait", projectedWait ?? (object)DBNull.Value);
                itemCmd.Parameters.AddWithValue("iconName", item.Icon);
                itemCmd.Parameters.AddWithValue("iconColour", item.ColorHex);
                itemCmd.Parameters.AddWithValue("attractionId", item.AttractionId ?? (object)DBNull.Value);

                await itemCmd.ExecuteNonQueryAsync();
            }
            
            foreach (var item in plan.FlexibleItems)
            {
                await using var itemCmd = new NpgsqlCommand(insertItemSql, conn, tx);
                itemCmd.Parameters.AddWithValue("itineraryId", plan.Id);
                itemCmd.Parameters.AddWithValue("time", DBNull.Value);
                itemCmd.Parameters.AddWithValue("title", item.Title);
                itemCmd.Parameters.AddWithValue("subtitle", item.Subtitle);
                
                // ⭐️ Safely extract the wait time using TryParse!
                var digitString = string.Join("", item.Subtitle.Where(char.IsDigit));
                int? projectedWait = null;
                
                if (!string.IsNullOrEmpty(digitString) && int.TryParse(digitString, out var parsedWait))
                {
                    projectedWait = parsedWait;
                }

                itemCmd.Parameters.AddWithValue("wait", projectedWait ?? (object)DBNull.Value);
                itemCmd.Parameters.AddWithValue("iconName", item.IconName);
                itemCmd.Parameters.AddWithValue("iconColour", item.ColorHex);
                itemCmd.Parameters.AddWithValue("attractionId", item.AttractionId ?? (object)DBNull.Value);

                await itemCmd.ExecuteNonQueryAsync();
            }

            // 4. Commit the transaction!
            await tx.CommitAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await tx.RollbackAsync();
            throw;
        }
    }
    
    public async Task<List<SavedPlanDto>> GetUserPlansAsync(string appUserId)
    {
        var plansDict = new Dictionary<Guid, SavedPlanDto>();
        
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // Grab the plans and their items in one swoop!
        const string sql = @"
            SELECT i.""Id"", i.""Title"", i.""TripDate"", i.""DestinationName"", i.""ParkName"", i.""ArrivalTime"", i.""DepartureTime"",
                   ii.""Id"" as ItemId, ii.""ScheduledTime"", ii.""CustomTitle"", ii.""CustomSubtitle"", ii.""IconName"", ii.""IconColour"", ii.""AttractionId"",
                   a.""ParkId"",
                   i.""ShareCode"",
                   CASE WHEN i.""AppUserId"" = @userId THEN true ELSE false END AS IsOwner
            FROM ""Users"".""Itinerary"" i
            LEFT JOIN ""Users"".""ItineraryItem"" ii ON i.""Id"" = ii.""ItineraryId""
            LEFT JOIN ""Static"".""Attraction"" a ON ii.""AttractionId"" = a.""AttractionId""
            WHERE i.""AppUserId"" = @userId 
            OR i.""Id"" IN (SELECT ""ItineraryId"" FROM ""Users"".""ItineraryMember"" WHERE ""AppUserId"" = @userId)
            ORDER BY i.""TripDate"" ASC, ii.""ScheduledTime"" ASC";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("userId", appUserId);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var planId = reader.GetGuid(0);
            
            // If we haven't seen this plan yet, create it!
            if (!plansDict.TryGetValue(planId, out var plan))
            {
                plan = new SavedPlanDto
                {
                    Id = planId,
                    Title = reader.GetString(1),
                    TripDate = reader.GetDateTime(2),
                    DestinationName = reader.GetString(3),
                    ParkName = reader.GetString(4),
                    ArrivalTime = reader.IsDBNull(5) ? null : reader.GetString(5),
                    DepartureTime = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Items = new List<PlanItemDto>(),
                    FlexibleItems = new List<FlexibleItemDto>(),
                    ParkId = reader.GetString(14),
                    ShareCode = reader.GetString(15),
                    IsOwner = reader.GetBoolean(16),
                };
                plansDict.Add(planId, plan);
            }

            // If there's an attached itinerary item, map it!
            if (!reader.IsDBNull(7)) 
            {
                if (!reader.IsDBNull(8))
                {

                    plan.Items.Add(new PlanItemDto
                    {
                        Id = reader.GetGuid(7),
                        Time = DateTime.Today.Add(reader.GetTimeSpan(8)).ToString("hh:mm tt"),
                        Title = reader.GetString(9),
                        Subtitle = reader.GetString(10),
                        Icon = reader.GetString(11),
                        ColorHex = reader.GetString(12),
                        AttractionId = reader.IsDBNull(13) ? null : reader.GetString(13)
                    });
                }
                else
                {
                    plan.FlexibleItems.Add(new FlexibleItemDto()
                    {
                        Id = reader.GetGuid(7),
                        Title = reader.GetString(9),
                        Subtitle = reader.GetString(10),
                        IconName = reader.GetString(11),
                        ColorHex = reader.GetString(12),
                        AttractionId = reader.IsDBNull(13) ? null : reader.GetString(13)
                    });
                }
            }
            
            
        }

        // Calculate the total rides for the summary cards
        foreach (var p in plansDict.Values)
        {
            p.TotalActivities = p.Items.Count;
        }

        return plansDict.Values.ToList();
    }
    
    public async Task<SavedPlanDto?> GetPlanByIdAsync(string planId, string appUserId)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // ⭐️ 1. The SQL focuses on a single ID, but keeps the security check!
        const string sql = @"
            SELECT i.""Id"", i.""Title"", i.""TripDate"", i.""DestinationName"", i.""ParkName"", i.""ArrivalTime"", i.""DepartureTime"",
                   ii.""Id"" as ItemId, ii.""ScheduledTime"", ii.""CustomTitle"", ii.""CustomSubtitle"", ii.""IconName"", ii.""IconColour"", ii.""AttractionId"",
                   a.""ParkId"",
                   i.""ShareCode"",
                   CASE WHEN i.""AppUserId"" = @userId THEN true ELSE false END AS IsOwner
            FROM ""Users"".""Itinerary"" i
            LEFT JOIN ""Users"".""ItineraryItem"" ii ON i.""Id"" = ii.""ItineraryId""
            LEFT JOIN ""Static"".""Attraction"" a ON ii.""AttractionId"" = a.""AttractionId""
            WHERE i.""Id"" = @planId 
            ORDER BY ii.""ScheduledTime"" ASC";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("planId", NpgsqlTypes.NpgsqlDbType.Uuid, Guid.Parse(planId));
        cmd.Parameters.AddWithValue("userId", appUserId);

        await using var reader = await cmd.ExecuteReaderAsync();
        
        // ⭐️ 2. No dictionary needed! Just a single variable we build over the rows.
        SavedPlanDto? plan = null;

        while (await reader.ReadAsync())
        {
            // On the very first row, we create the Plan object.
            if (plan == null)
            {
                plan = new SavedPlanDto
                {
                    Id = reader.GetGuid(0),
                    Title = reader.GetString(1),
                    TripDate = reader.GetDateTime(2),
                    DestinationName = reader.GetString(3),
                    ParkName = reader.GetString(4),
                    ArrivalTime = reader.IsDBNull(5) ? null : reader.GetString(5),
                    DepartureTime = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Items = new List<PlanItemDto>(),
                    FlexibleItems = new List<FlexibleItemDto>(),
                    ParkId = reader.GetString(14),
                    ShareCode = reader.GetString(15),
                    IsOwner = reader.GetBoolean(16),
                };
            }

            // ⭐️ 3. On EVERY row, we check for an item and append it
            if (!reader.IsDBNull(7)) 
            {
                if (!reader.IsDBNull(8)) // Fixed Time Item
                {
                    plan.Items.Add(new PlanItemDto
                    {
                        Id = reader.GetGuid(7),
                        Time = DateTime.Today.Add(reader.GetTimeSpan(8)).ToString("hh:mm tt"),
                        Title = reader.GetString(9),
                        Subtitle = reader.GetString(10),
                        Icon = reader.GetString(11),
                        ColorHex = reader.GetString(12),
                        AttractionId = reader.IsDBNull(13) ? null : reader.GetString(13)
                    });
                }
                else // Flexible Item
                {
                    plan.FlexibleItems.Add(new FlexibleItemDto()
                    {
                        Id = reader.GetGuid(7),
                        Title = reader.GetString(9),
                        Subtitle = reader.GetString(10),
                        IconName = reader.GetString(11),
                        ColorHex = reader.GetString(12),
                        AttractionId = reader.IsDBNull(13) ? null : reader.GetString(13)
                    });
                }
            }
        }

        // 4. Calculate the total before returning
        if (plan != null)
        {
            plan.TotalActivities = plan.Items.Count;
        }

        // Will return null if the plan doesn't exist or the user doesn't have permission!
        return plan;
    }

    public async Task<SavedPlanDto?> GetPlanPreviewByShareCodeAsync(string shareCode)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // ⭐️ THE FIX: No massive JOINs! Just the parent data and a quick count.
        const string sql = @"
            SELECT ""Id"", ""Title"", ""TripDate"", ""DestinationName"", ""ParkName"", ""ArrivalTime"", ""DepartureTime"", ""ShareCode"",
                   (SELECT COUNT(*) FROM ""Users"".""ItineraryItem"" WHERE ""ItineraryId"" = i.""Id"") as ActivityCount
            FROM ""Users"".""Itinerary"" i
            WHERE i.""ShareCode"" = @shareCode;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("shareCode", shareCode.ToUpper());

        await using var reader = await cmd.ExecuteReaderAsync();
        
        if (!await reader.ReadAsync()) 
        {
            return null; // The code was invalid!
        }

        // ⭐️ Map just the basic info for the UI Preview Sheet
        return new SavedPlanDto
        {
            Id = reader.GetGuid(0),
            Title = reader.GetString(1),
            TripDate = reader.GetDateTime(2),
            DestinationName = reader.GetString(3),
            ParkName = reader.GetString(4),
            ArrivalTime = reader.IsDBNull(5) ? null : reader.GetString(5),
            DepartureTime = reader.IsDBNull(6) ? null : reader.GetString(6),
            ShareCode = reader.GetString(7),
            TotalActivities = reader.GetInt32(8), // 👈 Mapped perfectly from our subquery!
            
            // We deliberately leave these empty to save payload size!
            Items = new List<PlanItemDto>(),
            FlexibleItems = new List<FlexibleItemDto>() 
        };
    }

    public async Task DeletePlanAsync(string appUserId, Guid planId)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // The CASCADE constraint handles deleting all the itinerary items automatically!
        const string sql = @"
        DELETE FROM ""Users"".""Itinerary"" 
        WHERE ""Id"" = @planId AND ""AppUserId"" = @userId;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("planId", planId);
        cmd.Parameters.AddWithValue("userId", appUserId);

        await cmd.ExecuteNonQueryAsync();
    }
    
    public async Task RenamePlanAsync(string appUserId, Guid planId, string newTitle)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // Just update the title, keeping it scoped to the specific user!
        const string sql = @"
        UPDATE ""Users"".""Itinerary"" 
        SET ""Title"" = @title 
        WHERE ""Id"" = @id AND ""AppUserId"" = @userId;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("title", newTitle);
        cmd.Parameters.AddWithValue("id", planId);
        cmd.Parameters.AddWithValue("userId", appUserId);

        await cmd.ExecuteNonQueryAsync();
    }
    
    public async Task<bool> JoinPlanByShareCodeAsync(string appUserId, string shareCode)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // 1. Find the parent Plan ID based on the Share Code
        const string findSql = @"
            SELECT ""Id"" 
            FROM ""Users"".""Itinerary"" 
            WHERE ""ShareCode"" = @shareCode;";
            
        await using var findCmd = new NpgsqlCommand(findSql, conn);
        findCmd.Parameters.AddWithValue("shareCode", shareCode.ToUpper());
        
        var result = await findCmd.ExecuteScalarAsync();
        
        if (result == null) return false; // The code was invalid!
        
        var planId = (Guid)result;

        // 2. Insert the relationship into our Junction Table!
        // We use ON CONFLICT DO NOTHING so the app doesn't crash if they tap "Join" twice.
        const string joinSql = @"
            INSERT INTO ""Users"".""ItineraryMember"" (""ItineraryId"", ""AppUserId"") 
            VALUES (@planId, @userId)
            ON CONFLICT DO NOTHING;"; 
            
        await using var joinCmd = new NpgsqlCommand(joinSql, conn);
        joinCmd.Parameters.AddWithValue("planId", planId);
        joinCmd.Parameters.AddWithValue("userId", appUserId);
        
        await joinCmd.ExecuteNonQueryAsync();

        return true;
    }
    
    public async Task<bool> LeavePlanAsync(string appUserId, Guid planId)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // ⭐️ One clean query to sever the link!
        const string sql = @"
            DELETE FROM ""Users"".""ItineraryMember"" 
            WHERE ""ItineraryId"" = @planId AND ""AppUserId"" = @userId;";
            
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("planId", planId);
        cmd.Parameters.AddWithValue("userId", appUserId);
        
        // ExecuteNonQueryAsync returns the number of rows affected.
        // If it's greater than 0, it means they successfully left the plan!
        var rowsAffected = await cmd.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }
}