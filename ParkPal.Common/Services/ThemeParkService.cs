using System.Text.Json;
using ParkPal.Common.API;
using ParkPal.Common.Models;
using ParkPal.Common.Models.Enums;
using ParkPal.Common.Services.Interfaces;

namespace ParkPal.Common.Services;

public class ThemeParkService(ThemeParkApi _api) : IThemeParkService
{
    public async Task<List<Destination>> GetDestinationsAsync()
    {
        List<Destination> destinations = []; 
        
        var response = await _api.GetDestinationsAsync();

        if (response?.Destinations != null)
        {
            foreach (var destination in response.Destinations)
            {
                var destinationEntityData = await _api.GetEntityDataAsync(destination.Id);
                
                if (destination.Parks != null && destination.Parks.Any() && destinationEntityData != null)
                {
                    Destination destinationToAdd = new(destination.Id, destination.Name)
                    {
                        Timezone = destinationEntityData.Timezone,
                        Longitude = destinationEntityData.Location.Longitude, 
                        Latitude = destinationEntityData.Location.Latitude
                    };

                    foreach (var park in destination.Parks)
                    {
                        var parkEntityData = await _api.GetEntityDataAsync(park.Id);

                        Park parkToAdd = new(park.Id, park.Name)
                        {
                            Latitude = parkEntityData?.Location.Latitude,
                            Longitude = parkEntityData?.Location.Longitude,
                        };
                        
                        destinationToAdd.Parks.Add(parkToAdd);
                    }
                    destinations.Add(destinationToAdd);
                }
            }
        }

        return destinations;
    }
    
    public async Task<List<Park>> GetDestinationsParksAsync(string destinationId)
    {
        List<Park> parks = [];
        var childrenResponse = await _api.GetChildrenAsync(destinationId);

        if (childrenResponse?.Children != null)
        {
            foreach (var park in childrenResponse.Children.Where(a => a.EntityType == EntityType.PARK)) 
            {
                parks.Add(new Park(park.Id, park.Name));
            }
        }

        return parks;
    }
    
    public async Task<Destination?> GetDestinationWithParksAsync(string destinationId)
    {
        var childrenResponse = await _api.GetChildrenAsync(destinationId);

        if (childrenResponse != null)
        {
            Destination destination = new(childrenResponse.Id, childrenResponse.Name);

            if (childrenResponse.Children != null)
            {
                foreach (var park in childrenResponse.Children.Where(a => a.EntityType == EntityType.PARK))
                {
                    destination.Parks.Add(new Park(park.Id, park.Name));
                }
            }

            return destination;
        }

        return null;
    }
    
    public async Task<EntityLiveDataResponse?> GetParkWaitTimesAsync(string parkId)
    {
        return await _api.GetWaitTimesAsync(parkId);
    }
    
    public async Task<EntityChildrenResponse?> GetParkChildrenAttractionsAsync(string parkId)
    {
        return await _api.GetChildrenAsync(parkId);
    }

    public async Task<Park?> GetParkWithAttractionsAsync(string parkId)
    {
        var parkDataFromApi = await GetParkWaitTimesAsync(parkId);
        var parkChildrenResponse = await GetParkChildrenAttractionsAsync(parkId);
        var parkChildren = parkChildrenResponse?.Children;

        if (parkChildren == null) return null;

        var park = new Park(parkId, parkDataFromApi?.Name ?? "Unknown Park");

        // ⭐️ Loop over CHILDREN, not LiveData!
        foreach (var child in parkChildren)
        {
            // Try to find matching live data (might be null for restaurants!)
            var liveData = parkDataFromApi?.LiveData?.FirstOrDefault(l => l.Id == child.Id);

            var rawStandby = liveData?.Queue?.STANDBY?.WaitTime;
            var safeStandby = rawStandby.HasValue ? (int)rawStandby.Value : (int?)null;
            var safeType = child.EntityType;
            
            // If there's no live data (e.g. Restaurant), default to Operating
            var status = liveData != null 
                ? DetermineParkPalStatus(liveData.Status, rawStandby.HasValue, safeType) 
                : ParkPalAttractionStatus.Operating;

            // Serialize showtimes if they exist
            var showtimesJson = liveData?.Showtimes != null 
                ? JsonSerializer.Serialize(liveData.Showtimes) 
                : null;
            
            // We need to be storing the live data as JSON, rather than 
            var liveDataJson = liveData != null ? JsonSerializer.Serialize(liveData) : null;

            var parkPalAttraction = new AttractionDto(child.Id, child.Name, child.EntityType, status)
            {
                WaitTime = safeStandby,
                LastUpdated = liveData?.LastUpdated,
                SingleRiderWaitTime = liveData?.Queue?.SINGLE_RIDER?.WaitTime.HasValue == true 
                    ? (int)liveData.Queue.SINGLE_RIDER.WaitTime.Value : null,
                LightningLaneReturnStart = liveData?.Queue?.PAID_RETURN_TIME?.ReturnStart 
                                        ?? liveData?.Queue?.RETURN_TIME?.ReturnStart,
                IsVirtualQueueOnly = liveData?.Queue is { BOARDING_GROUP: not null, STANDBY: null },
                HasActiveAlert = false,
                
                // The new data!
                ExternalId = child.ExternalId,
                Latitude = child.Location?.Latitude,
                Longitude = child.Location?.Longitude,
                ShowtimesJson = showtimesJson,
                // We will store the live data JSON in the history table for the attraction.  
                LiveDataJson = liveDataJson,
            };

            park.Attractions.Add(parkPalAttraction);
        }

        return park;
    }

    // ⭐️ THE UPGRADED HELPER: Now it knows about Entity Types!
    private ParkPalAttractionStatus DetermineParkPalStatus(LiveStatusType apiStatus, bool hasStandbyTime, EntityType entityType)
    {
        if (apiStatus == LiveStatusType.DOWN) return ParkPalAttractionStatus.Down;
        if (apiStatus == LiveStatusType.CLOSED) return ParkPalAttractionStatus.Closed;

        // ⭐️ The "Ghost Ride" Rule: ONLY applies to RIDES!
        if (entityType == EntityType.ATTRACTION && apiStatus == LiveStatusType.OPERATING && !hasStandbyTime)
            return ParkPalAttractionStatus.Closed;

        return ParkPalAttractionStatus.Operating;
    }

    public async Task<List<AttractionDto>> GetParkAttractionsAsync(string parkId)
    {
        List<AttractionDto> attractions = [];
        var parkDataFromApi = await GetParkWaitTimesAsync(parkId);

        if (parkDataFromApi?.LiveData != null)
        {
            foreach (var attractionData in parkDataFromApi.LiveData)
            {
                double? rawWaitTime = attractionData.Queue?.STANDBY?.WaitTime;

                // ⭐️ Check HasValue here so we know it's safe to cast inside the block
                if (rawWaitTime.HasValue && attractionData.EntityType == EntityType.ATTRACTION && attractionData.Status != LiveStatusType.REFURBISHMENT)
                {
                    var status = ParkPalAttractionStatus.Operating;
                    int safeWaitTime = (int)rawWaitTime.Value;
                    
                    attractions.Add(new AttractionDto(attractionData.Id, attractionData.Name, EntityType.ATTRACTION, status)
                    {
                        WaitTime =  safeWaitTime,
                    });
                }
            }
        }

        return attractions;
    }
    
    // ⭐️ Returning an int? so your background worker has clean data
    public async Task<int?> GetAttractionWaitTimeAsync(string parkId, string attractionId)
    {
        var response = await GetParkWaitTimesAsync(parkId);
        var attraction = response?.LiveData?.FirstOrDefault(a => a.Id == attractionId);
        
        double? rawWaitTime = attraction?.Queue?.STANDBY?.WaitTime;
        
        return rawWaitTime.HasValue ? (int)rawWaitTime.Value : null;
    }
}