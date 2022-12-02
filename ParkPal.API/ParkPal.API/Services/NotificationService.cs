using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ParkPal.API.Models;
using ParkPal.API.Models.OneSignal.Requests;
using ParkPal.API.Models.Responses;
using ParkPal.API.Services.Interfaces;
using ParkPal.Common.API.Models.ThemeParkApi;
using ParkPal.Common.Database.Contexts;
using ParkPal.Common.Models;
using ParkPal.Common.Models.Database.Entities.Notification;
using ParkPal.Common.Models.Database.Entities.Notification.Enums;
using ParkPal.Common.Services;
using ParkPal.Common.Services.Interfaces;
using RestSharp;
using Type = ParkPal.Common.Models.Database.Entities.Notification.Enums.Type;

namespace ParkPal.API.Services;

public class NotificationService: INotificationService
{
    private readonly DatabaseContext _context;
    private IThemeParkService _themeParkService;
    
    public NotificationService(DatabaseContext context, IThemeParkService themeParkService)
    {
        _context = context;
        _themeParkService = themeParkService;
    }

    public List<Notification> GetAllNotifications(string token)
    {
        List<Notification> notifications = new List<Notification>();
        
        Subscription? subscription =
            _context.Subscriptions
                .Include(a => a.Token)
                .FirstOrDefault(a => a.Token != null && a.Token.Value == token);

        if (subscription != null)
        {
            // Get the notifications.
            List<Item> databaseNotifications = _context.Notifications.Include(a => a.Subscription).Where(a =>
                a.Subscription.SubscriptionId == subscription.SubscriptionId).ToList();
            
            // Get all parkIds in our timers.                                             
            List<string> parkIds = databaseNotifications.Select(a => a.ParkId).Distinct().ToList();
            Dictionary<string, Park?> parksLiveData = new();

            // Loop through parkId we have notifications for and get the park wait times for each one from the API.
            foreach (string parkId in parkIds)
            {
                // We sleep for 5 seconds so that we aren't hammering the API.
                parksLiveData.Add(parkId, _themeParkService.GetParkWithAttractions(parkId));
            }

            foreach (Item notification in databaseNotifications)
            {
                // Get the park for the notification.
                Park? parkData = parksLiveData[notification.ParkId];
                Attraction? attraction = null;
                
                if (notification.TypeId != (int)Type.Park)
                {
                    attraction =
                        parkData?.Attractions.FirstOrDefault(a => a.AttractionId == notification.AttractionId);
                }
                
                Notification apiNotification = new Notification()
                {
                    Properties = notification,
                    Attraction = attraction,
                    Park = parkData
                };
                    
                notifications.Add(apiNotification);
            }
        }

        return notifications;
    }

    public Item? GetNotification(string playerId, string attractionId, string parkId)
    {
        return _context.Notifications.Include(a => a.Subscription).FirstOrDefault(a =>
            a.Subscription.PlayerId == playerId && a.AttractionId == attractionId && a.ParkId == parkId);
    }
    
    public Item? CreateNotification(string token, Type type, string attractionId, string parkId,
        CriteriaType criteriaType, int waitTime, int minuteInterval = 5)
    {
        // We will only receive the token from the header, use that to get the subscriptionId.
        Subscription? subscription =
            _context.Subscriptions
                .Include(a => a.Token)
                .FirstOrDefault(a => a.Token != null && a.Token.Value == token);

        if (subscription != null)
        {
            Item notification = new Item()
            {
                SubscriptionId = subscription.SubscriptionId,
                TypeId = (int)type,
                AttractionId = attractionId,
                CriteriaType = (int)criteriaType,
                ParkId = parkId,
                WaitTime = waitTime,
                MinuteInterval = minuteInterval,
                Enabled = true
            };

            _context.Notifications.Add(notification);
            _context.SaveChanges();

            return notification;
        }

        return null;
    }

    public Item? EditNotification(int notificationId, CriteriaType criteriaType, int waitTime)
    {
        Item? notification =
            _context.Notifications.FirstOrDefault(a => a.ItemId == notificationId);

        if (notification != null)
        {
            notification.CriteriaType = (int)criteriaType;
            notification.WaitTime = waitTime;

            _context.SaveChanges();
            return notification;
        }

        return null;
    }

    public Item? DisableNotification(int notificationId)
    {
        return SetEnabledFlag(notificationId, false);
    }

    public Item? EnableNotification(int notificationId)
    {
        return SetEnabledFlag(notificationId, true);
    }

    public bool DeleteNotification(int notificationId)
    {
        Item? notification =
            _context.Notifications.FirstOrDefault(a => a.ItemId == notificationId);

        if (notification != null)
        {
            _context.Notifications.Remove(notification);
            _context.SaveChanges();

            return true;
        }

        return false;
    }

    public Item? SetEnabledFlag(int notificationId, bool enabled)
    {
        Item? notification =
            _context.Notifications.FirstOrDefault(a => a.ItemId == notificationId);

        if (notification != null)
        {
            notification.Enabled = enabled;
            _context.SaveChanges();

            return notification;
        }

        return null;
    }
}