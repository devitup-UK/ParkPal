using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkPal.API.Models;
using ParkPal.API.Models.Enums;
using ParkPal.API.Models.Requests.Notification;
using ParkPal.API.Models.Requests.Subscription;
using ParkPal.API.Models.Requests.Token;
using ParkPal.API.Models.Responses.Notification;
using ParkPal.API.Services.Interfaces;
using ParkPal.Common.Models.Database.Entities.Device;
using ParkPal.Common.Models.Database.Entities.Notification;

namespace ParkPal.API.Controllers;

[Authorize]
[ApiController]
[Route("notification")]
public class NotificationController : ControllerBase
{
    private INotificationService _notificationService;
    private ITokenService _tokenService;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(ILogger<NotificationController> logger, INotificationService notificationService, ITokenService tokenService)
    {
        _logger = logger;
        _notificationService = notificationService;
        _tokenService = tokenService;
    }
    
    [HttpPost("")]
    public IActionResult Notifications([FromBody] GetNotificationsRequest request)
    {
        List<TimerWithAttraction> timers = new List<TimerWithAttraction>();

        string token = User.FindFirstValue(ClaimTypes.Name);

        timers = _notificationService.GetAllTimers(token);
        
        // Get all timers by only the specified parkId if one is set in the filters.
        if (request.Filters.ParkId != null)
        {
            timers = timers.FindAll(a => a.Park.ParkId == request.Filters.ParkId);
        }

        // Get all timers by any criteria set, this should default to All.
        switch (request.Filters.Criteria)
        {
            case NotificationsFilterCriteria.LessThan:
            case NotificationsFilterCriteria.MoreThan:
            case NotificationsFilterCriteria.EqualTo:
                timers = timers.FindAll(a => a.Timer.CriteriaType == (int)request.Filters.Criteria);
                break;
        }
        
        // Now filter any notifications by our type, we only need to cover Favourites for now but this could change in the future.
        switch (request.Filters.Type)
        {
            case NotificationsFilterType.Favourites:
                timers = timers.FindAll(a => request.FavouriteAttractionIds.Contains(a.Attraction.AttractionId));
                break;
        }
        
        // Finally, we sort the timers by the request sorting value.
        switch (request.Filters.Sort)
        {
            case NotificationsFilterSort.ThrillRides:
                timers = timers.OrderByDescending(a => a.Attraction.Thrill).ToList();
                break;
            case NotificationsFilterSort.TameRides:
                timers = timers.OrderByDescending(a => !a.Attraction.Thrill).ToList();
                break;
            case NotificationsFilterSort.HighestWaitTime:
                timers = timers.OrderByDescending(a => a.Attraction.WaitTime).ToList();
                break;
            case NotificationsFilterSort.LowestWaitTime:
                timers = timers.OrderBy(a => a.Attraction.WaitTime).ToList();
                break;
        }
        
        return Ok(timers);
    }
    
    [HttpPost("create")]
    public IActionResult Create([FromBody] CreateNotificationRequest request)
    {
        string token = User.FindFirstValue(ClaimTypes.Name);

        AttractionTimer? timer = _notificationService.CreateTimer(token, request.AttractionId, request.ParkId, request.CriteriaType,
            request.WaitTime);

        if (timer != null)
        {
            return Ok(timer);
        }

        return Problem();
    }
    
    [HttpPost("edit")]
    public IActionResult Edit([FromBody] EditNotificationRequest request)
    {
        string token = User.FindFirstValue(ClaimTypes.Name);

        AttractionTimer? timer = _notificationService.EditTimer(request.AttractionTimerId, request.CriteriaType,
            request.WaitTime);

        if (timer != null)
        {
            return Ok(timer);
        }

        return Problem();
    }
    
    [HttpPost("enable")]
    public IActionResult Enable([FromBody] EnableDisableNotificationRequest request)
    {
        string token = User.FindFirstValue(ClaimTypes.Name);
        
        AttractionTimer? timer = _notificationService.EnableTimer(request.AttractionTimerId);

        if (timer.Enabled)
        {
            return Ok(timer);
        }

        return Problem();
    }
    
    [HttpPost("disable")]
    public IActionResult Disable([FromBody] EnableDisableNotificationRequest request)
    {
        string token = User.FindFirstValue(ClaimTypes.Name);
        
        AttractionTimer? timer = _notificationService.DisableTimer(request.AttractionTimerId);

        if (!timer.Enabled)
        {
            return Ok(timer);
        }

        return Problem();
    }
    
    [HttpDelete("delete/{attractionTimerId}")]
    public IActionResult Delete(int attractionTimerId)
    {
        bool deleted = _notificationService.DeleteTimer(attractionTimerId);

        if (deleted)
        {
            return Ok(deleted);
        }

        return UnprocessableEntity(deleted);
    }
}