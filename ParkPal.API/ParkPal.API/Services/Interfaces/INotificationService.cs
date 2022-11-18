using ParkPal.API.Models.OneSignal.Requests;
using ParkPal.API.Models.Responses.Notification;
using ParkPal.Common.Models.Database.Entities.Notification;
using ParkPal.Common.Models.Database.Entities.Notification.Enums;

namespace ParkPal.API.Services.Interfaces;

public interface INotificationService
{
    public List<TimerWithAttraction> GetAllTimers(string token);
    public AttractionTimer? GetTimer(string playerId, string attractionId, string parkId);
    public AttractionTimer? CreateTimer(string token, string attractionId, string parkId,
        CriteriaType criteriaType, int waitTime, int minuteInterval = 5);

    public AttractionTimer? EditTimer(int attractionTimerId,
        CriteriaType criteriaType,
        int waitTime);

    public AttractionTimer? DisableTimer(int attractionTimerId);
    public AttractionTimer? EnableTimer(int attractionTimerId);
    public AttractionTimer? SetEnabledFlag(int attractionTimerId, bool enabled);

    public bool DeleteTimer(int attractionTimerId);
}