using ParkPal.Common.API.Models.ThemeParkApi;
using ParkPal.Common.Models;

namespace ParkPal.Common.Services.Interfaces;

public interface IThemeParkService
{
    public List<Destination> GetDestinations();
    public List<Park> GetDestinationsParks(string destinationId);
    public Destination? GetDestinationWithParks(string destinationId);
    public Park? GetParkWithAttractions(string parkId);
    public EntityLiveDataResponse? GetParkWaitTimes(string parkId);
    public List<Attraction> GetParkAttractions(string parkId);
    public List<EntityLiveData> GetAttractionWaitTimes(string parkId, string attractionId);
    public int? GetAttractionWaitTime(string attractionId, List<EntityLiveData> listOfAttractionDetails);
}