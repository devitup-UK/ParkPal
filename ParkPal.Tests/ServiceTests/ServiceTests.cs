using System;
using System.IO.Enumeration;
using System.Threading;
using System.Timers;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using ParkPal.API.Models;
using ParkPal.API.Services;
using ParkPal.API.Services.Interfaces;
using ParkPal.Common.Models.Configuration;
using Timer = System.Timers.Timer;

namespace ParkPal.Tests.ServiceTests;

public class ServiceTests
{
    private IOptions<AppSettings> _appSettings;
    private static Timer notificationTimer;
    
    [SetUp]
    public void Setup()
    {
        AppSettings appSettings = new AppSettings()
        {
            HostEnvironment = "dev",
            ClientToken = "WZJ9BGXfVZITT48zHxIqjxzl3vb1MMRH0R6WoZ6pn2qnPFGtiLE7q5bp2CCeu7F7",
            KeyVaultUrl = "https://vault.devitup.co.uk"
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