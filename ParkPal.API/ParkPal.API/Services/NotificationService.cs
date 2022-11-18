using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ParkPal.API.Models;
using ParkPal.API.Models.OneSignal.Requests;
using ParkPal.API.Models.Responses.Notification;
using ParkPal.API.Services.Interfaces;
using ParkPal.Common.API.Models.ThemeParkApi;
using ParkPal.Common.Database.Contexts;
using ParkPal.Common.Models;
using ParkPal.Common.Models.Database.Entities.Notification;
using ParkPal.Common.Models.Database.Entities.Notification.Enums;
using ParkPal.Common.Services;
using ParkPal.Common.Services.Interfaces;
using RestSharp;

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

    public List<TimerWithAttraction> GetAllTimers(string token)
    {
        List<TimerWithAttraction> timers = new List<TimerWithAttraction>();
        
        Subscription? subscription =
            _context.Subscriptions
                .Include(a => a.Token)
                .FirstOrDefault(a => a.Token != null && a.Token.Value == token);

        if (subscription != null)
        {
            // Get the attraction timers.
            List<AttractionTimer> attractionTimerNotifications = _context.AttractionTimers.Include(a => a.Subscription).Where(a =>
                a.Subscription.SubscriptionId == subscription.SubscriptionId).ToList();
            
            // Get all parkIds in our timers.                                             
            List<string> parkIds = attractionTimerNotifications.Select(a => a.ParkId).Distinct().ToList();
            Dictionary<string, Park> parksLiveData = new();

            // Loop through parkId we have notifications for and get the park wait times for each one from the API.
            foreach (string parkId in parkIds)
            {
                // We sleep for 5 seconds so that we aren't hammering the API.
                parksLiveData.Add(parkId, _themeParkService.GetParkWithAttractions(parkId));
            }

            foreach (AttractionTimer timer in attractionTimerNotifications)
            {
                // Get the park for the notification.
                Park parkData = parksLiveData[timer.ParkId];
                Attraction? attraction = parkData.Attractions.FirstOrDefault(a => a.AttractionId == timer.AttractionId);

                if (attraction != null)
                {
                    TimerWithAttraction timerWithAttraction = new TimerWithAttraction()
                    {
                        Timer = timer,
                        Attraction = attraction,
                        Park = parkData
                    };
                    
                    timers.Add(timerWithAttraction);
                }
            }
        }

        return timers;
    }

    public AttractionTimer? GetTimer(string playerId, string attractionId, string parkId)
    {
        return _context.AttractionTimers.Include(a => a.Subscription).FirstOrDefault(a =>
            a.Subscription.PlayerId == playerId && a.AttractionId == attractionId && a.ParkId == parkId);
    }
    
    public AttractionTimer? CreateTimer(string token, string attractionId, string parkId,
        CriteriaType criteriaType, int waitTime, int minuteInterval = 5)
    {
        // We will only receive the token from the header, use that to get the subscriptionId.
        Subscription? subscription =
            _context.Subscriptions
                .Include(a => a.Token)
                .FirstOrDefault(a => a.Token != null && a.Token.Value == token);

        if (subscription != null)
        {
            AttractionTimer newTimer = new AttractionTimer()
            {
                SubscriptionId = subscription.SubscriptionId,
                AttractionId = attractionId,
                CriteriaType = (int)criteriaType,
                ParkId = parkId,
                WaitTime = waitTime,
                MinuteInterval = minuteInterval,
                Enabled = true
            };

            _context.AttractionTimers.Add(newTimer);
            _context.SaveChanges();

            return newTimer;
        }

        return null;
    }

    public AttractionTimer? EditTimer(int attractionTimerId, CriteriaType criteriaType, int waitTime)
    {
        AttractionTimer? attractionTimer =
            _context.AttractionTimers.FirstOrDefault(a => a.AttractionTimerId == attractionTimerId);

        if (attractionTimer != null)
        {
            attractionTimer.CriteriaType = (int)criteriaType;
            attractionTimer.WaitTime = waitTime;

            _context.SaveChanges();
            return attractionTimer;
        }

        return null;
    }

    public AttractionTimer? DisableTimer(int attractionTimerId)
    {
        return SetEnabledFlag(attractionTimerId, false);
    }

    public AttractionTimer? EnableTimer(int attractionTimerId)
    {
        return SetEnabledFlag(attractionTimerId, true);
    }

    public bool DeleteTimer(int attractionTimerId)
    {
        AttractionTimer? attractionTimer =
            _context.AttractionTimers.FirstOrDefault(a => a.AttractionTimerId == attractionTimerId);

        if (attractionTimer != null)
        {
            _context.AttractionTimers.Remove(attractionTimer);
            _context.SaveChanges();

            return true;
        }

        return false;
    }

    public AttractionTimer? SetEnabledFlag(int attractionTimerId, bool enabled)
    {
        AttractionTimer? attractionTimer =
            _context.AttractionTimers.FirstOrDefault(a => a.AttractionTimerId == attractionTimerId);

        if (attractionTimer != null)
        {
            attractionTimer.Enabled = enabled;
            _context.SaveChanges();

            return attractionTimer;
        }

        return null;
    }
}