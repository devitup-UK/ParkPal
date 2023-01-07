using Microsoft.EntityFrameworkCore;
using ParkPal.Common.API.Models.ThemeParkApi;
using ParkPal.Common.Database.Contexts;
using ParkPal.Common.Models.Database.Entities.Notification;
using ParkPal.Common.Models.Database.Entities.Notification.Enums;
using ParkPal.Common.Services.Interfaces;
using ParkPal.NotificationService.Services.Interfaces;

namespace ParkPal.NotificationService.BackgroundServices;

public class TimerService : IHostedService, IDisposable
{
    private Timer? _timer;
    private readonly ILogger<TimerService> _logger;
    private readonly DatabaseContext _context;
    private readonly IThemeParkService _themeParkService;
    private readonly IOneSignalService _oneSignalService;

    public TimerService(ILogger<TimerService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _context = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<DatabaseContext>();
        _themeParkService = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IThemeParkService>();
        _oneSignalService = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IOneSignalService>();
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Notification Service running, checking every 5 minutes for park times");

        _timer = new Timer(ProcessNotifications, null, TimeSpan.Zero,
            TimeSpan.FromMinutes(5));

        return Task.CompletedTask;
    }

    private void ProcessNotifications(object? state)
    {
        _logger.LogInformation("Scanning the database for matching notifications to send to the users");
        
        // Get all of the notifications we need to handle in the database.
        List<Item> notifications = _context.Notifications.Include(a => a.Subscription).Where(a => a.Enabled).ToList();
        
        _logger.LogInformation("{Count} attraction notification timers to process", notifications.Count);
        
        // Then we need to get all of the park Id's from our original query.
        List<string> parkIds = notifications.Select(a => a.ParkId).Distinct().ToList();
        // Now we will create a list of dictionary to store our wait time data against our parkId. 
        Dictionary<string, EntityLiveDataResponse> parksLiveData = new();
        
        _logger.LogInformation("Fetching wait times for {ParkIdsCount} parks", parkIds.Count);
        
        // Loop through parkId we have notifications for and get the park wait times for each one from the API.
        foreach (string parkId in parkIds)
        {
            // We sleep for 5 seconds so that we aren't hammering the API.
            Thread.Sleep(5000);
            EntityLiveDataResponse? parkData = _themeParkService.GetParkWaitTimes(parkId);
            if (parkData != null)
            {
                parksLiveData.Add(parkId, parkData);
            }
        }

        foreach (Item notification in notifications)
        {
            // Get the live data response for this specific park for this notification.
            EntityLiveDataResponse parkResponseForNotification = parksLiveData[notification.ParkId];

            if (notification.AttractionId != null)
            {
                EntityLiveData? attractionData =
                    parkResponseForNotification.LiveData.FirstOrDefault(a =>
                        a.Id == notification.AttractionId && a.Status != LiveStatusType.DOWN &&
                        a.Status != LiveStatusType.CLOSED && a.Status != LiveStatusType.REFURBISHMENT);

                if (attractionData != null)
                {
                    // Get the wait time of the attraction that this notification is for.
                    int? attractionWaitTime =
                        _themeParkService.GetAttractionWaitTime(notification.AttractionId,
                            parkResponseForNotification.LiveData);

                    // If the attraction wait time is not null, we can continue.
                    if (attractionWaitTime != null)
                    {
                        switch (notification.CriteriaType)
                        {
                            case (int)CriteriaType.EqualTo:
                                if (attractionWaitTime == notification.WaitTime)
                                {
                                    if (attractionWaitTime == 0 || attractionWaitTime == 1)
                                    {
                                        _oneSignalService.SendPushNotificationToPlayer("ParkPal",
                                            $"There is currently no wait time for {attractionData.Name}.",
                                            notification.Subscription.PlayerId);
                                    }
                                    else
                                    {
                                        _oneSignalService.SendPushNotificationToPlayer("ParkPal",
                                            $"The wait time for {attractionData.Name} is at {attractionWaitTime} minutes right now.",
                                            notification.Subscription.PlayerId);
                                    }
                                }

                                break;
                            case (int)CriteriaType.LessThan:
                                if (attractionWaitTime <= notification.WaitTime)
                                {
                                    if (attractionWaitTime == 0 || attractionWaitTime == 1)
                                    {
                                        _oneSignalService.SendPushNotificationToPlayer("ParkPal",
                                            $"There is currently no wait time for {attractionData.Name}.",
                                            notification.Subscription.PlayerId);
                                    }
                                    else
                                    {
                                        _oneSignalService.SendPushNotificationToPlayer("ParkPal",
                                            $"The wait time for {attractionData.Name} is at {attractionWaitTime} minutes right now.",
                                            notification.Subscription.PlayerId);
                                    }
                                }

                                break;
                            case (int)CriteriaType.MoreThan:
                                if (attractionWaitTime >= notification.WaitTime)
                                {
                                    if (attractionWaitTime == 0 || attractionWaitTime == 1)
                                    {
                                        _oneSignalService.SendPushNotificationToPlayer("ParkPal",
                                            $"There is currently no wait time for {attractionData.Name}.",
                                            notification.Subscription.PlayerId);
                                    }
                                    else
                                    {
                                        _oneSignalService.SendPushNotificationToPlayer("ParkPal",
                                            $"The wait time for {attractionData.Name} is at {attractionWaitTime} minutes right now.",
                                            notification.Subscription.PlayerId);
                                    }
                                }

                                break;
                        }
                    }
                }
            }
            else
            {
                int attractionsThatMatchCriteraCount = 0;
                EntityLiveData? closestAttractionToCriteria = null;
                List<EntityLiveData> attractions = new List<EntityLiveData>();
                string description = "less than";
                bool equalTo = false;
                
                // This is a park specific notification.
                // foreach (EntityLiveData attractionData in parkResponseForNotification.LiveData)
                // {
                    switch (notification.CriteriaType)
                    {
                        case (int)CriteriaType.LessThan:
                            description = "less than";
                            attractions = parkResponseForNotification.LiveData
                                .Where(a => a.Queue?.STANDBY?.WaitTime <= notification.WaitTime).OrderBy(a => a.Queue?.STANDBY?.WaitTime).ToList();
                            break;
                        case (int)CriteriaType.MoreThan:
                            description = "more than";
                            attractions = parkResponseForNotification.LiveData
                                .Where(a => a.Queue?.STANDBY?.WaitTime >= notification.WaitTime).OrderByDescending(a => a.Queue?.STANDBY?.WaitTime).ToList();
                            break;
                        case (int)CriteriaType.EqualTo:
                            equalTo = true;
                            description = "at";
                            attractions = parkResponseForNotification.LiveData
                                .Where(a => a.Queue?.STANDBY?.WaitTime == notification.WaitTime).OrderByDescending(a => a.Queue?.STANDBY?.WaitTime).ToList();
                            break;
                    }
                    
                    if (attractions.Any())
                    {
                        attractionsThatMatchCriteraCount = attractions.Count;
                        closestAttractionToCriteria = attractions[0];
                        int? lowestWaitTime = closestAttractionToCriteria.Queue?.STANDBY?.WaitTime;

                        if (lowestWaitTime != null)
                        {
                            if (!equalTo)
                            {
                                _oneSignalService.SendPushNotificationToPlayer("ParkPal",
                                    $"{parkResponseForNotification.Name} has {attractionsThatMatchCriteraCount} attractions that are {description} {notification.WaitTime} minutes right now. The attraction with the lowest wait time is {closestAttractionToCriteria.Name} with {lowestWaitTime} minutes.",
                                    notification.Subscription.PlayerId);
                            }
                            else
                            {
                                _oneSignalService.SendPushNotificationToPlayer("ParkPal",
                                    $"{parkResponseForNotification.Name} has {attractionsThatMatchCriteraCount} attractions that are {description} {notification.WaitTime} minutes right now.",
                                    notification.Subscription.PlayerId);
                            }
                            
                        }
                        else
                        {
                            _oneSignalService.SendPushNotificationToPlayer("ParkPal",
                                $"{parkResponseForNotification.Name} has {attractionsThatMatchCriteraCount} that are {description} {notification.WaitTime} minutes right now. Check out the Wait Times to see a list of matching attractions.",
                                notification.Subscription.PlayerId);
                        }
                        
                        
                    }
                // }
                
            }
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Notification Service is stopping, no more notifications will be sent");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}