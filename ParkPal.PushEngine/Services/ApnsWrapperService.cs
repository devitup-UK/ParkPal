using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options; // ⭐️ Required for IOptions
using CorePush.Apple;
using ParkPal.PushEngine.Models;

namespace ParkPal.PushEngine.Services;

public class ApnsWrapper
{
    private readonly ApplePushSettings _settings; // ⭐️ Holds our dynamic secrets
    private readonly ApnSender _alertSender;
    private readonly HttpClient _liveActivityClient;
    private string _cachedJwt = string.Empty;
    private DateTime _jwtGeneratedAt = DateTime.MinValue;

    // ⭐️ Inject the settings via the constructor
    public ApnsWrapper(ApplePushSettings settings)
    {
        _settings = settings;
        var sharedHttp = new HttpClient(); 
        
        // Setup CorePush for Standard Alerts
        var alertSettings = new ApnSettings
        {
            AppBundleIdentifier = settings.AppBundleId,
            P8PrivateKey = settings.P8PrivateKey,
            P8PrivateKeyId = settings.P8KeyId,
            TeamId = settings.TeamId,
            ServerType = settings.UseProductionServers ? ApnServerType.Production : ApnServerType.Development 
        };
        
        _alertSender = new ApnSender(alertSettings, sharedHttp);

        var handler = new SocketsHttpHandler
        {
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(20),
            KeepAlivePingDelay = TimeSpan.FromSeconds(60),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
            EnableMultipleHttp2Connections = true,
            SslOptions = new System.Net.Security.SslClientAuthenticationOptions
            {
                EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13
            }
        };

        _liveActivityClient = new HttpClient(handler);
    }

    // ====================================================================
    // STANDARD ALERTS (Uses CorePush)
    // ====================================================================
    public async Task<bool> SendWaitTimeAlertAsync(string deviceToken, string title, string bodyMessage)
    {
        var payload = new AppleNotification
        {
            Aps = new AppleNotification.ApsPayload
            {
                Alert = new AppleNotification.AlertPayload
                {
                    Title = title,
                    Body = bodyMessage
                },
                Sound = "default",
                Badge = 1
            }
        };

        var response = await _alertSender.SendAsync(payload, deviceToken);
        return response.IsSuccessStatusCode;
    }

    // ====================================================================
    // LIVE ACTIVITIES (Uses Raw HttpClient)
    // ====================================================================
    public async Task<bool> SendLiveActivityUpdateAsync(string deviceToken, int currentWaitTime, string status)
    {
        var payload = new LiveActivityNotification
        {
            Aps = new LiveActivityAps
            {
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Event = "update",
                ContentState = new LiveActivityContentState
                {
                    CurrentWaitTime = currentWaitTime,
                    Status = status
                }
            }
        };

        // ⭐️ Dynamically swap the URL between Sandbox and Production
        var appleDomain = _settings.UseProductionServers ? "api.push.apple.com" : "api.sandbox.push.apple.com";
        var request = new HttpRequestMessage(HttpMethod.Post, $"https://{appleDomain}:2197/3/device/{deviceToken.Trim()}");

        request.Version = new Version(2, 0);
        request.VersionPolicy = HttpVersionPolicy.RequestVersionExact;

        request.Headers.Add("apns-push-type", "liveactivity");
        request.Headers.Add("apns-topic", $"{_settings.AppBundleId}.push-type.liveactivity"); 
        request.Headers.Add("apns-priority", "10"); 
        
        request.Headers.Authorization = new AuthenticationHeaderValue("bearer", GetOrGenerateJwt());
        var jsonPayload = JsonSerializer.Serialize(payload);
        var httpContent = new ByteArrayContent(Encoding.UTF8.GetBytes(jsonPayload));
        request.Content = httpContent;

        var response = await _liveActivityClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"❌ Live Activity Push Failed: {error}");
        }

        return response.IsSuccessStatusCode;
    }

    // ====================================================================
    // 3. PURE APPLE AUTHENTICATION LOGIC
    // ====================================================================
    private string GetOrGenerateJwt()
    {
        if (!string.IsNullOrEmpty(_cachedJwt) && (DateTime.UtcNow - _jwtGeneratedAt).TotalMinutes < 45) return _cachedJwt;

        var header = new { alg = "ES256", kid = _settings.P8KeyId };
        var payload = new { iss = _settings.TeamId, iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds() };

        var headerBase64 = Base64UrlEncode(JsonSerializer.Serialize(header));
        var payloadBase64 = Base64UrlEncode(JsonSerializer.Serialize(payload));
        var unsignedJwt = $"{headerBase64}.{payloadBase64}";

        using var ecdsa = ECDsa.Create();
        ecdsa.ImportPkcs8PrivateKey(Convert.FromBase64String(_settings.P8PrivateKey), out _);
        
        var signatureBytes = ecdsa.SignData(Encoding.UTF8.GetBytes(unsignedJwt), HashAlgorithmName.SHA256);
        var signatureBase64 = Base64UrlEncode(signatureBytes);

        _cachedJwt = $"{unsignedJwt}.{signatureBase64}";
        _jwtGeneratedAt = DateTime.UtcNow;

        return _cachedJwt;
    }

    private static string Base64UrlEncode(string input) => Base64UrlEncode(Encoding.UTF8.GetBytes(input));
    private static string Base64UrlEncode(byte[] input) => Convert.ToBase64String(input).Replace("+", "-").Replace("/", "_").TrimEnd('=');
}

// ====================================================================
// 📦 HELPER MODELS FOR THE LIVE ACTIVITY PAYLOAD
// ====================================================================

public class LiveActivityNotification
{
    [JsonPropertyName("aps")]
    public LiveActivityAps Aps { get; set; } = new();
}

public class LiveActivityAps
{
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    [JsonPropertyName("event")]
    public string Event { get; set; } = "update";

    // ⭐️ The magic hyphen that makes Apple happy!
    [JsonPropertyName("content-state")]
    public LiveActivityContentState ContentState { get; set; } = new();
}

public class LiveActivityContentState
{
    // These must perfectly match the spelling and casing of your Swift struct!
    [JsonPropertyName("currentWaitTime")]
    public int CurrentWaitTime { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}