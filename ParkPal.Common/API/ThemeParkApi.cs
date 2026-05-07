namespace ParkPal.Common.API;

// ⭐️ Primary constructor passes the injected HttpClient down to the BaseApi
public class ThemeParkApi(HttpClient client) : BaseApi(client)
{
    public async Task<DestinationsResponse?> GetDestinationsAsync()
    {
        return await GetRequestAsync<DestinationsResponse>("destinations");
    }
    
    public async Task<EntityData?> GetEntityDataAsync(string entityIdOrSlug)
    {
        return await GetRequestAsync<EntityData>($"entity/{entityIdOrSlug}");
    }

    public async Task<EntityChildrenResponse?> GetChildrenAsync(string entityIdOrSlug)
    {
        return await GetRequestAsync<EntityChildrenResponse>($"entity/{entityIdOrSlug}/children");
    }

    public async Task<EntityLiveDataResponse?> GetWaitTimesAsync(string entityIdOrSlug)
    {
        return await GetRequestAsync<EntityLiveDataResponse>($"entity/{entityIdOrSlug}/live");
    }
}