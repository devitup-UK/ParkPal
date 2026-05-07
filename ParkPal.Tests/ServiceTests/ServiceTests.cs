using Microsoft.Extensions.Options;
using NUnit.Framework;
using ParkPal.API.Models;
using Timer = System.Timers.Timer;

namespace ParkPal.Tests.ServiceTests;

public class ServiceTests
{
    private IOptions<AppSettingsConfiguration> _appSettings;
    private static Timer notificationTimer;
    
    [SetUp]
    public void Setup()
    {
        var appSettings = new AppSettingsConfiguration()
        {
        };

        _appSettings = Options.Create(appSettings);
    }

    // [Test]
    // public void SendBasicPushNotification()
    // {
    //     INotificationService notificationService = new NotificationService(_appSettings);
    //
    //     bool success = notificationService.SendPushNotification("3deb1872-fa73-41a1-aa9d-2d3657a98cac", "Hello",
    //         "Sent Via C#, how crazy!");
    //     
    //     Assert.IsTrue(success);
    // }

    // [Test]
    // public void SendRepeatedPushNotification()
    // {
    //     notificationTimer = new Timer();
    //     notificationTimer.Interval = 20000;
    //
    //     notificationTimer.Elapsed += SendTimerNotification;
    //     notificationTimer.Enabled = true;
    //     
    //     System.Threading.Thread.Sleep(65000);
    // }
    //
    // private void SendTimerNotification(Object source, ElapsedEventArgs e)
    // {
    //     SendBasicPushNotification();
    // }
}