import Attraction from "@/models/api/Attraction";
import Park from "@/models/api/Park";
import {ActionSheetButton, actionSheetController} from "@ionic/vue";
import NotificationHoldingArea from "@/models/store/NotificationHoldingArea";
import {hideBannerAdvertisement, resumeBannerAdvertisement} from "@/handlers/advertisements.handler";
import Notification from "@/models/api/Notification";
import store from "@/store";
import router from "@/router";
import {NotificationType} from "@/models/enums/NotificationType";

export async function configureNotification(attraction: Attraction, park: Park, notificationAttractionIds: Array<string>, destinationId: string, notifications: Array<Notification>) {
    const buttons: Array<ActionSheetButton> = [];

    if(attraction.attractionId) {

        if (!notificationAttractionIds.includes(attraction.attractionId)) {
            buttons.push({
                text: 'Create Wait Time Notification',
                data: {
                    action: 'share',
                },
                handler: () => {
                    store.commit('setNotificationHoldingArea', new NotificationHoldingArea({attraction, park, type: NotificationType.Attraction}));

                    router.push({
                        name: 'notificationsCreate',
                        params: {
                            destinationId,
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
                    store.commit('setNotificationHoldingArea', new NotificationHoldingArea({attraction, park, type: NotificationType.Attraction}));

                    await router.push({
                        name: 'notificationsEdit',
                        params: {
                            notificationId: notifications.filter((a: Notification) => a.properties?.attractionId == attraction.attractionId)[0].properties?.itemId,
                            transition: 'slide-right'
                        }
                    })
                    await store.dispatch('setModalOpen', false);
                    resumeBannerAdvertisement(store.state.settings.parkPalPlus);
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
                header: attraction.name,
                buttons: buttons
            });

            await store.dispatch('setModalOpen', true);
            hideBannerAdvertisement();

            await actionSheet.present();

        }

        await presentActionSheet();
    }
}