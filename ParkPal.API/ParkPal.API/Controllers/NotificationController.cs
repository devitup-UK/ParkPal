using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkPal.API.Models;
using ParkPal.API.Models.Enums;
using ParkPal.API.Models.Requests.Notification;
using ParkPal.API.Models.Requests.Subscription;
using ParkPal.API.Models.Requests.Token;
using ParkPal.API.Models.Responses;
using ParkPal.API.Services.Interfaces;
using ParkPal.Common.Models.Database.Entities.Device;
using ParkPal.Common.Models.Database.Entities.Notification;
using Type = ParkPal.Common.Models.Database.Entities.Notification.Enums.Type;

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
        string token = User.FindFirstValue(ClaimTypes.Name);

        List<Notification> notifications = _notificationService.GetAllNotifications(token);
        
        // Get all timers by only the specified parkId if one is set in the filters.
        if (request.Filters.ParkId != null)
        {
            notifications = notifications.FindAll(a => a.Park?.ParkId == request.Filters.ParkId);
        }

        // Get all timers by any criteria set, this should default to All.
        switch (request.Filters.Criteria)
        {
            case NotificationsFilterCriteria.LessThan:
            case NotificationsFilterCriteria.MoreThan:
            case NotificationsFilterCriteria.EqualTo:
                notifications = notifications.FindAll(a => a.Properties?.CriteriaType == (int)request.Filters.Criteria);
                break;
        }
        
        // Now filter any notifications by our type, we only need to cover Favourites for now but this could change in the future.
        switch (request.Filters.Type)
        {
            case NotificationsFilterType.Favourites:
                notifications = notifications.FindAll(a => (request.FavouriteIds.Contains(a.Attraction?.AttractionId) && a.Properties.TypeId == (int)Type.Attraction) || (request.FavouriteIds.Contains(a.Park?.ParkId) && a.Properties.TypeId == (int)Type.Park));
                break;
            case NotificationsFilterType.Attractions:
                notifications = notifications.FindAll(a => a.Properties.TypeId == (int)Type.Attraction);
                break;
            case NotificationsFilterType.Parks:
                notifications = notifications.FindAll(a => a.Properties.TypeId == (int)Type.Park);
                break;
        }
        
        // Finally, we sort the timers by the request sorting value.
        switch (request.Filters.Sort)
        {
            case NotificationsFilterSort.ThrillRides:
                notifications = notifications.Where(a => a.Properties.TypeId == (int)Type.Attraction).OrderByDescending(a => a.Attraction?.Thrill).ToList();
                break;
            case NotificationsFilterSort.TameRides:
                notifications = notifications.Where(a => a.Properties.TypeId == (int)Type.Attraction).OrderByDescending(a => !a.Attraction?.Thrill).ToList();
                break;
            case NotificationsFilterSort.HighestWaitTime:
                notifications = notifications.OrderByDescending(a => a.Properties.WaitTime).ToList();
                break;
            case NotificationsFilterSort.LowestWaitTime:
                notifications = notifications.OrderBy(a => a.Properties?.WaitTime).ToList();
                break;
        }
        
        return Ok(notifications);
    }
    
    [HttpPost("create")]
    public IActionResult Create([FromBody] CreateNotificationRequest request)
    {
        string token = User.FindFirstValue(ClaimTypes.Name);

        Item? notification = _notificationService.CreateNotification(token, request.Type, request.AttractionId, request.ParkId, request.CriteriaType,
            request.WaitTime);

        if (notification != null)
        {
            return Ok(notification);
        }

        return Problem();
    }
    
    [HttpPost("edit")]
    public IActionResult Edit([FromBody] EditNotificationRequest request)
    {
        string token = User.FindFirstValue(ClaimTypes.Name);

        Item? notification = _notificationService.EditNotification(request.NotificationId, request.CriteriaType,
            request.WaitTime);

        if (notification != null)
        {
            return Ok(notification);
        }

        return Problem();
    }
    
    [HttpPost("enable")]
    public IActionResult Enable([FromBody] EnableDisableNotificationRequest request)
    {
        string token = User.FindFirstValue(ClaimTypes.Name);
        
        Item? notification = _notificationService.EnableNotification(request.NotificationId);

        if (notification.Enabled)
        {
            return Ok(notification);
        }

        return Problem();
    }
    
    [HttpPost("disable")]
    public IActionResult Disable([FromBody] EnableDisableNotificationRequest request)
    {
        string token = User.FindFirstValue(ClaimTypes.Name);
        
        Item? notification = _notificationService.DisableNotification(request.NotificationId);

        if (!notification.Enabled)
        {
            return Ok(notification);
        }

        return Problem();
    }
    
    [HttpDelete("delete/{notificationId}")]
    public IActionResult Delete(int notificationId)
    {
        bool deleted = _notificationService.DeleteNotification(notificationId);

        if (deleted)
        {
            return Ok(deleted);
        }

        return UnprocessableEntity(deleted);
    }
}