import OneSignal from "onesignal-cordova-plugin";
import {subscriptionService} from "@/services/subscription.service";
import {IOSSettings, NativeSettings} from 'capacitor-native-settings';
import store from "@/store";

export async function setupOneSignal() {
    return new Promise((resolve, reject) => {
        // NOTE: Update the setAppId value below with your OneSignal AppId.
        OneSignal.setAppId("9260f4f6-44b4-4dfa-b67b-a52d9a86a7f3");
        OneSignal.setNotificationOpenedHandler(function(jsonData) {
            // This is what happens when someone clicks a notification.
            console.log('notificationOpenedCallback: ' + JSON.stringify(jsonData));
        });

        resolve(true);
    })
}

export async function requestNotificationPermissions() {
    return new Promise((resolve, reject) => {
        // Prompts the user for notification permissions.
        //    * Since this shows a generic native prompt, we recommend instead using an In-App Message to prompt for notification permission (See step 7) to better communicate to your users what notifications they will get.
        OneSignal.promptForPushNotificationsWithUserResponse(async (accepted: boolean) => {
            if(accepted) {
                resolve(accepted);
            }else{
                reject('Push Notifications rejected.');
            }
        });
    })
}

export async function saveSubscriptionToDatabase() {
    // The user has accepted, so now we can send our player Id to our backend.
    return new Promise((resolve,reject) => {
        OneSignal.getDeviceState((state) => {
            if(state.userId) {
                // Send a request to save the player Id in the database for the user with this token.
                subscriptionService.save({ playerId: state.userId}).then((response) => {
                    resolve(response);
                });
            }else{
                reject();
            }
        })
    });
}

export async function openAppNotificationSettings() {
    await NativeSettings.openIOS({
        option: IOSSettings.App
    })
}