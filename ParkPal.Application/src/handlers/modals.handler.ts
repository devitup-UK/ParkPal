import Attraction from "@/models/api/Attraction";
import Park from "@/models/api/Park";
import {ActionSheetButton, actionSheetController} from "@ionic/vue";
import NotificationHoldingArea from "@/models/store/NotificationHoldingArea";
import {hideBannerAdvertisement, resumeBannerAdvertisement} from "@/handlers/advertisements.handler";
import Notification from "@/models/api/Notification";
import store from "@/store";
import router from "@/router";
import {NotificationType} from "@/models/enums/NotificationType";

export async function configureNotification(attraction: Attraction, park: Park, notificationIds: Array<string>, notifications: Array<Notification>, type: NotificationType) {
    const buttons: Array<ActionSheetButton> = [];
    let notificationId = 0;
    let notificationEntityId = '';
    let header = '';


    if(type == NotificationType.Attraction) {
        notificationEntityId = attraction.attractionId;
        header = attraction.name;

        const attractionNotification = notifications.find((a: Notification) => a.properties?.attractionId == attraction.attractionId);
        if(attractionNotification) {
            notificationId = attractionNotification.properties.itemId;
        }
    }else{
        notificationEntityId = park.parkId;
        header = park.name;

        const parkNotification = notifications.find((a: Notification) => !a.attraction.attractionId.length && a.properties?.parkId == park.parkId);
        if(parkNotification) {
            notificationId = parkNotification.properties.itemId;
        }
    }


        if (!notificationIds.includes(notificationEntityId)) {
            buttons.push({
                text: 'Create Wait Time Notification',
                data: {
                    action: 'share',
                },
                handler: () => {
                    store.commit('setNotificationHoldingArea', new NotificationHoldingArea({attraction, park, type}));

                    router.push({
                        name: 'notificationsCreate',
                        params: {
                            transition: 'slide-right'
                        }
                    })

                    resumeBannerAdvertisement(store.state.settings.parkPalPlus);
                }
            })
        } else {
            buttons.push({
                text: 'Edit Wait Time Notification',
                data: {
                    action: 'share',
                },
                handler: async () => {
                    store.commit('setNotificationHoldingArea', new NotificationHoldingArea({attraction, park, type}));

                    await router.push({
                        name: 'notificationsEdit',
                        params: {
                            notificationId,
                            transition: 'slide-right'
                        }
                    })
                    await store.dispatch('setModalOpen', false);
                    resumeBannerAdvertisement(store.state.settings.parkPalPlus);
                }
            })

            buttons.push({
                text: 'Delete Wait Time Notification',
                role: 'destructive',
                handler: async () => {
                    await deleteNotification(notificationId);
                }
            })
        }

        buttons.push({
            text: 'Cancel',
            role: 'cancel',
            data: {
                action: 'cancel',
            },
            handler: async () => {
                await store.dispatch('setModalOpen', false);
                resumeBannerAdvertisement(store.state.settings.parkPalPlus);
            }
        });

        const presentActionSheet = async () => {
            const actionSheet = await actionSheetController.create({
                header,
                buttons: buttons
            });

            await store.dispatch('setModalOpen', true);
            hideBannerAdvertisement();

            await actionSheet.present();

        }

        await presentActionSheet();
    // }
}

export async function deleteNotification(notificationId: number) {
    await store.dispatch('deleteNotification', notificationId);
}