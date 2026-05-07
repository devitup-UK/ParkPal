using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using CorePush.Apple;
using ParkPal.PushEngine.Models;

namespace ParkPal.PushEngine.Services;

public class ApnsWrapper
{
    private const string P8PrivateKey = "MIGTAgEAMBMGByqGSM49AgEGCCqGSM49AwEHBHkwdwIBAQQg26hYLoiD1lzE244Vfs0iUc/f71icHYz/0XBF2Nn8UdagCgYIKoZIzj0DAQehRANCAAT/sUNlk+QowBEZ5EdtrfApTmNE3zvCEKkAVUUq2ShKMIo8QaGIj/JaJvDIsphx5W/WyAMXaPNMUD2zrMfNOL/W";
    private const string P8KeyId = "3WN46MZD2H";
    private const string TeamId = "8Q972PMJ52";
    private const string AppBundleId = "DevItUp.App.ParkPal";

    // 1. The CorePush Sender (For Alerts)
    private readonly ApnSender _alertSender;
    
    // 2. The Raw HTTP Client (For Live Activities)
    private readonly HttpClient _liveActivityClient;
    
    // 3. The JWT Cache
    private string _cachedJwt = string.Empty;
    private DateTime _jwtGeneratedAt = DateTime.MinValue;

    public ApnsWrapper()
    {
        var sharedHttp = new HttpClient(); 
        
        // Setup CorePush for Standard Alerts
        var alertSettings = new ApnSettings
        {
            AppBundleIdentifier = AppBundleId,
            P8PrivateKey = P8PrivateKey,
            P8PrivateKeyId = P8KeyId,
            TeamId = TeamId,
            ServerType = ApnServerType.Development 
        };
        _alertSender = new ApnSender(alertSettings, sharedHttp);

        var handler = new SocketsHttpHandler
        {
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(20),
            KeepAlivePingDelay = TimeSpan.FromSeconds(60),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
            EnableMultipleHttp2Connections = true,
            // 🚨 THE FIX: Force TLS 1.2 or 1.3! If it tries anything lower, Apple hangs up.
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

        // 🚨 THE FIX: Use Port 2197!
        var request = new HttpRequestMessage(HttpMethod.Post, $"https://api.sandbox.push.apple.com:2197/3/device/{deviceToken.Trim()}");

        // 🚨 THE FIX: Force HTTP/2 on the Request itself, not just the client!
        request.Version = new Version(2, 0);
        request.VersionPolicy = HttpVersionPolicy.RequestVersionExact;

        request.Headers.Add("apns-push-type", "liveactivity");
        request.Headers.Add("apns-topic", $"{AppBundleId}.push-type.liveactivity"); 
        request.Headers.Add("apns-priority", "10"); 
        
        request.Headers.Authorization = new AuthenticationHeaderValue("bearer", GetOrGenerateJwt());
        var jsonPayload = JsonSerializer.Serialize(payload);
        var jsonBytes = Encoding.UTF8.GetBytes(jsonPayload);
        var httpContent = new ByteArrayContent(jsonBytes);
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
        if (!string.IsNullOrEmpty(_cachedJwt) && (DateTime.UtcNow - _jwtGeneratedAt).TotalMinutes < 45)
        {
            return _cachedJwt;
        }

        // 1. Apple strictly forbids ANY claims other than 'iss' and 'iat'
        var header = new { alg = "ES256", kid = P8KeyId };
        var payload = new { iss = TeamId, iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds() };

        var headerBase64 = Base64UrlEncode(JsonSerializer.Serialize(header));
        var payloadBase64 = Base64UrlEncode(JsonSerializer.Serialize(payload));

        var unsignedJwt = $"{headerBase64}.{payloadBase64}";

        // 2. Sign it directly with the curve Apple demands
        using var ecdsa = ECDsa.Create();
        ecdsa.ImportPkcs8PrivateKey(Convert.FromBase64String(P8PrivateKey), out _);
        
        var signatureBytes = ecdsa.SignData(Encoding.UTF8.GetBytes(unsignedJwt), HashAlgorithmName.SHA256);
        var signatureBase64 = Base64UrlEncode(signatureBytes);

        _cachedJwt = $"{unsignedJwt}.{signatureBase64}";
        _jwtGeneratedAt = DateTime.UtcNow;

        return _cachedJwt;
    }

    // Standard base64 URL encoding required for JWTs!
    private static string Base64UrlEncode(string input) => Base64UrlEncode(Encoding.UTF8.GetBytes(input));
    
    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }
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