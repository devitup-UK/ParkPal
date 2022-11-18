using ParkPal.Common.API;
using ParkPal.Common.API.Models.ThemeParkApi;
using ParkPal.Common.API.Models.ThemeParkApi.Enums;
using ParkPal.Common.Models;
using ParkPal.Common.Models.Configuration;
using ParkPal.Common.Models.Enums;
using ParkPal.Common.Services.Interfaces;

namespace ParkPal.Common.Services;

public class ThemeParkService: IThemeParkService
{
    private readonly ThemeParkApi _api;
    
    public ThemeParkService()
    {
        _api = new ThemeParkApi(Settings.ThemeParkWaitTimeUrl);
    }
    
    public List<Destination> GetDestinations()
    {
        List<Destination> destinations = new();
        
        ThemeParkApi api = new(Settings.ThemeParkWaitTimeUrl);
        
        // This returns a list of destinations back from the API.
        DestinationsResponse? destinationsResponse = api.GetDestinations();

        if (destinationsResponse != null)
        {
            foreach (DestinationEntry destination in destinationsResponse.Destinations)
            {
                if (destination.Parks.Any())
                {
                    Destination destinationToAdd =
                        new(destination.Id, destination.Name);

                    if (!destinationToAdd.Hidden)
                    {

                        foreach (DestinationParkEntry park in destination.Parks)
                        {
                            Park parkToAdd = new Park(park.Id, park.Name);
                            if (!parkToAdd.Hidden)
                            {
                                destinationToAdd.Parks.Add(parkToAdd);
                            }
                        }

                        destinations.Add(destinationToAdd);
                    }
                }
            }
        }

        return destinations;
    }
    
    public List<Park> GetDestinationsParks(string destinationId)
    {
        List<Park> parks = new();
        
        EntityChildrenResponse? childrenResponse = _api.GetChildren(destinationId);

        if (childrenResponse != null)
        {
            foreach (EntityChild park in childrenResponse.Children.Where(a => a.EntityType == EntityType.PARK))
            {
                parks.Add(new Park(park.Id, park.Name));
            }
        }

        return parks;
    }
    
    public Destination? GetDestinationWithParks(string destinationId)
    {
        EntityChildrenResponse? childrenResponse = _api.GetChildren(destinationId);

        if (childrenResponse != null)
        {

            Destination destination = new Destination(childrenResponse.Id, childrenResponse.Name);

            foreach (EntityChild park in childrenResponse.Children.Where(a => a.EntityType == EntityType.PARK))
            {
                destination.Parks.Add(new Park(park.Id, park.Name));
            }

            return destination;
        }

        return null;
    }
    
    public EntityLiveDataResponse? GetParkWaitTimes(string parkId)
    {
        return _api.GetWaitTimes(parkId);
    }

    public Park? GetParkWithAttractions(string parkId)
    {
        EntityLiveDataResponse? parkDataFromApi = GetParkWaitTimes(parkId);

        if (parkDataFromApi != null)
        {

            Park park = new(parkDataFromApi.Id, parkDataFromApi.Name);

            foreach (EntityLiveData attractionData in parkDataFromApi.LiveData)
            {
                if (attractionData.EntityType == EntityType.ATTRACTION &&
                    attractionData.Status != LiveStatusType.REFURBISHMENT)
                {
                    AttractionStatus status = (AttractionStatus)(int)attractionData.Status;

                    if (attractionData.Status == LiveStatusType.OPERATING &&
                        attractionData.Queue?.STANDBY?.WaitTime == null)
                    {
                        status = AttractionStatus.Closed;
                    }

                    park.Attractions.Add(new Attraction(attractionData.Id, attractionData.Name,
                        status, attractionData.Queue?.STANDBY?.WaitTime));
                }
            }

            return park;
        }

        return null;
    }

    public List<Attraction> GetParkAttractions(string parkId)
    {
        List<Attraction> attractions = new();
        
        EntityLiveDataResponse? parkDataFromApi = GetParkWaitTimes(parkId);

        if (parkDataFromApi != null)
        {

            foreach (EntityLiveData attractionData in parkDataFromApi.LiveData)
            {
                if (attractionData.Queue?.STANDBY?.WaitTime != null &&
                    attractionData.EntityType == EntityType.ATTRACTION &&
                    attractionData.Status != LiveStatusType.REFURBISHMENT)
                {
                    attractions.Add(new Attraction(attractionData.Id, attractionData.Name,
                        (AttractionStatus)(int)attractionData.Status, attractionData.Queue.STANDBY.WaitTime.Value));
                }
            }
        }

        return attractions;
    }
    
    public List<EntityLiveData> GetAttractionWaitTimes(string parkId, string attractionId)
    {
        EntityLiveDataResponse? response = GetParkWaitTimes(parkId);

        if (response != null)
        {
            return response.LiveData.Where(a => a.Id == attractionId).ToList();
        }

        return new List<EntityLiveData>();
    }

    public int? GetAttractionWaitTime(string attractionId, List<EntityLiveData> listOfAttractionDetails)
    {
        EntityLiveData? attractionQueue = listOfAttractionDetails.FirstOrDefault(a => a.Id == attractionId);

        return attractionQueue?.Queue?.STANDBY?.WaitTime;
    }
}