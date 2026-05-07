using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ParkPal.Common.API;

public class BaseApi
{
    // ⭐️ Use native HttpClient! 
    protected readonly HttpClient _client;
    protected readonly JsonSerializerOptions _jsonOptions;

    // ⭐️ Dependency Injection friendly constructor
    public BaseApi(HttpClient client)
    {
        _client = client;
        
        // This is crucial: APIs send lowercase JSON (e.g. "name"), but C# uses PascalCase ("Name").
        // This tells System.Text.Json to ignore the case differences automatically.
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() } // ⭐️ THIS IS THE MAGIC BULLET!
        };
    }

    protected async Task<T?> GetRequestAsync<T>(string endpoint) where T : class
    {
        try
        {
            // ⭐️ ONE line to fetch and deserialize! 
            return await _client.GetFromJsonAsync<T>(endpoint, _jsonOptions);
        }
        catch (HttpRequestException ex)
        {
            // TODO: Log with Serilog! Log.Error(ex, "API Call Failed");
            return null;
        }
    }

    // For future-proofing your POSTs
    protected async Task<T?> PostRequestAsync<T>(string endpoint, object payload) where T : class
    {
        var response = await _client.PostAsJsonAsync(endpoint, payload, _jsonOptions);
        
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        
        return null;
    }
}