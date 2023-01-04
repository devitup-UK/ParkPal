using ParkPal.Common.Models.Database.Entities.Device;
using ParkPal.Common.Models.Database.Entities.Notification;
using ParkPal.Common.Models.Database.Entities.Subscription;

namespace ParkPal.API.Services.Interfaces;

public interface ISubscriptionService
{
    Subscription GetByToken(string token);
    Subscription SaveSubscription(Token token, string playerId);
    bool OverwriteSubscription(Token token, string playerId);
    Voucher? RedeemVoucher(string code);
    bool VerifyVoucher(string code);
}