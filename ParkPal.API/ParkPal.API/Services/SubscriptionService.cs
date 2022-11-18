using Microsoft.EntityFrameworkCore;
using ParkPal.API.Services.Interfaces;
using ParkPal.Common.Database.Contexts;
using ParkPal.Common.Models.Database.Entities.Device;
using ParkPal.Common.Models.Database.Entities.Notification;

namespace ParkPal.API.Services;

public class SubscriptionService: ISubscriptionService
{
    private DatabaseContext _context;

    public SubscriptionService(DatabaseContext context)
    {
        _context = context;
    }

    public Subscription GetByToken(string token)
    {
        Token foundToken = _context.Tokens.FirstOrDefault(a => a.Value == token);
        if (foundToken != null)
        {
            return _context.Subscriptions.Include(a => a.Token).FirstOrDefault(a => a.TokenId == foundToken.TokenId);
        }

        return null;
    }

    public Subscription SaveSubscription(Token token, string playerId)
    {
        // Save the users subscription to the database.
        Subscription subscriptionToCreate = new Subscription()
        {
            TokenId = token.TokenId,
            PlayerId = playerId
        };

        _context.Subscriptions.Add(subscriptionToCreate);
        _context.SaveChanges();

        return subscriptionToCreate;
    }
    
    public bool OverwriteSubscription(Token token, string playerId)
    {
        // Save the users subscription to the database.
        Subscription subscriptionToOverwrite = _context.Subscriptions.FirstOrDefault(a => a.TokenId == token.TokenId);

        if (subscriptionToOverwrite != null)
        {
            subscriptionToOverwrite.PlayerId = playerId;
        }
        ;
        _context.SaveChanges();

        if (subscriptionToOverwrite?.TokenId == token.TokenId)
        {
            return true;
        }

        return false;
    }
}