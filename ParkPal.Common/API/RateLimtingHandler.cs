using System.Threading.RateLimiting;

namespace ParkPal.Common.API;

public class RateLimitingHandler : DelegatingHandler
{
    private readonly RateLimiter _rateLimiter;

    public RateLimitingHandler()
    {
        // ⭐️ The Token Bucket: 300 requests per minute!
        _rateLimiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
        {
            TokenLimit = 300, 
            TokensPerPeriod = 300, 
            ReplenishmentPeriod = TimeSpan.FromMinutes(1), 
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 1000, // How many requests can wait in line before we just throw an error
            AutoReplenishment = true
        });
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // ⭐️ 1. Ask the bouncer if we can enter (Wait here if we are over the limit!)
        using var lease = await _rateLimiter.AcquireAsync(1, cancellationToken);

        if (lease.IsAcquired)
        {
            // ⭐️ 2. We got a token! Send the actual HTTP request
            return await base.SendAsync(request, cancellationToken);
        }

        // ⭐️ 3. The queue is completely full (Over 1000 waiting). Reject to save memory.
        Console.WriteLine("🚨 Rate limit queue is completely full! Rejecting request.");
        return new HttpResponseMessage(System.Net.HttpStatusCode.TooManyRequests);
    }
}