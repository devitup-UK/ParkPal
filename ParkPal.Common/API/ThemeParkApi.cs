using ParkPal.Common.API.Enums;
using ParkPal.Common.API.Models.ThemeParkApi;

namespace ParkPal.Common.API;

public class ThemeParkApi: BaseApi
{
    public ThemeParkApi(string baseUrl) : base(baseUrl)
    {
    }

    public DestinationsResponse? GetDestinations()
    {
        return GetRequest<DestinationsResponse>("/destinations");
    }

    public EntityChildrenResponse? GetChildren(string entityIdOrSlug)
    {
        return GetRequest<EntityChildrenResponse>($"/entity/{entityIdOrSlug}/children");
    }

    public EntityLiveDataResponse? GetWaitTimes(string entityIdOrSlug)
    {
        return GetRequest<EntityLiveDataResponse>($"/entity/{entityIdOrSlug}/live");
    }
}