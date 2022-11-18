using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkPal.API.Models.Requests.Subscription;
using ParkPal.API.Models.Requests.Token;
using ParkPal.API.Services.Interfaces;
using ParkPal.Common.Models.Database.Entities.Device;
using ParkPal.Common.Models.Database.Entities.Notification;

namespace ParkPal.API.Controllers;

[Authorize]
[ApiController]
[Route("subscription")]
public class SubscriptionController : ControllerBase
{
    private ISubscriptionService _subscriptionService;
    private ITokenService _tokenService;
    private readonly ILogger<SubscriptionController> _logger;

    public SubscriptionController(ILogger<SubscriptionController> logger, ISubscriptionService subscriptionService, ITokenService tokenService)
    {
        _logger = logger;
        _subscriptionService = subscriptionService;
        _tokenService = tokenService;
    }
    
    // Step 1 - The app calls this endpoint if they have a token and we verify that it is legitimate.
    [HttpPost("save")]
    public IActionResult Save([FromBody]SaveSubscriptionRequest request)
    {
        string token = User.FindFirstValue(ClaimTypes.Name);
        if (!String.IsNullOrEmpty(token))
        {
            Subscription existingSubscription = _subscriptionService.GetByToken(token);
            Token tokenRow = _tokenService.GetByToken(token);

            if (existingSubscription != null)
            {

                if (existingSubscription.PlayerId != request.PlayerId)
                {
                    Subscription savedSubscription = _subscriptionService.SaveSubscription(tokenRow, request.PlayerId);
                    if (savedSubscription != null)
                    {
                        return Ok(savedSubscription);
                    }
                }

                return Ok(existingSubscription);
            }
            else
            {
                Subscription newSubscription = _subscriptionService.SaveSubscription(tokenRow, request.PlayerId);
                if (newSubscription != null)
                {
                    return Ok(newSubscription);
                }
            }
        }

        return NotFound();
    }
}