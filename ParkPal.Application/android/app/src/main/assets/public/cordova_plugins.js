
  cordova.define('cordova/plugin_list', function(require, exports, module) {
    module.exports = [
      {
          "id": "cordova-plugin-purchase.CdvPurchase",
          "file": "plugins/cordova-plugin-purchase/www/store.js",
          "pluginId": "cordova-plugin-purchase",
        "clobbers": [
          "store",
          "CdvPurchase"
        ]
        },
      {
          "id": "cordova-plugin-screen-orientation.screenorientation",
          "file": "plugins/cordova-plugin-screen-orientation/www/screenorientation.js",
          "pluginId": "cordova-plugin-screen-orientation",
        "clobbers": [
          "cordova.plugins.screenorientation"
        ]
        },
      {
          "id": "onesignal-cordova-plugin.OneSignalPlugin",
          "file": "plugins/onesignal-cordova-plugin/dist/index.js",
          "pluginId": "onesignal-cordova-plugin",
        "clobbers": [
          "OneSignal"
        ]
        },
      {
          "id": "es6-promise-plugin.Promise",
          "file": "plugins/es6-promise-plugin/www/promise.js",
          "pluginId": "es6-promise-plugin",
        "runs": true
        },
      {
          "id": "onesignal-cordova-plugin.NotificationReceivedEvent",
          "file": "plugins/onesignal-cordova-plugin/dist/NotificationReceivedEvent.js",
          "pluginId": "onesignal-cordova-plugin"
        },
      {
          "id": "onesignal-cordova-plugin.Subscription",
          "file": "plugins/onesignal-cordova-plugin/dist/Subscription.js",
          "pluginId": "onesignal-cordova-plugin"
        },
      {
          "id": "onesignal-cordova-plugin.OSNotification",
          "file": "plugins/onesignal-cordova-plugin/dist/OSNotification.js",
          "pluginId": "onesignal-cordova-plugin"
        }
    ];
    module.exports.metadata =
    // TOP OF METADATA
    {
      "cordova-annotated-plugin-android": "1.0.4",
      "cordova-plugin-purchase": "13.6.0",
      "cordova-plugin-screen-orientation": "3.0.2",
      "es6-promise-plugin": "4.2.2",
      "onesignal-cordova-plugin": "3.2.0"
    };
    // BOTTOM OF METADATA
    });
    