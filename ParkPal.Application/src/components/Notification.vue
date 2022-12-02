<template>
  <IonItemSliding class="notification" :class="{ 'notification--attraction': isAttractionNotification, 'notification--park': isParkNotification, 'notification--favourite': isFavourite }" @click="showNotificationOptions(attraction, park)" :style="`background: ${settings.theme.waitTimes.background} !important; color: ${settings.theme.waitTimes.text} !important;`" :disabled="notification.properties.itemId == 0">
    <h1 class="notification__type">{{ type }}</h1>
    <IonItem :style="`--border-radius: 16px; --background: ${settings.theme.waitTimes.background};--background-activated: ${settings.theme.waitTimes.background}; --background-focused: ${settings.theme.waitTimes.background}; --background-hover: ${settings.theme.waitTimes.background}; --highlight-color-focused: ${settings.theme.waitTimes.background};  --color: ${settings.theme.waitTimes.text}; --border-width: 0; --border-style: unset;`">
      <IonLabel>
        <!-- If this is an attraction notification, then we can show all of the attraction details and options. -->
        <template v-if="isAttractionNotification">
          <AttractionDetails :attraction="this.attraction"></AttractionDetails>
          <div class="notification__details" v-if="this.notification" :style="`background: ${this.settings.theme.waitTimes.text} !important;`">
            <p v-html="getNotificationMessage(notification)" :style="`color: ${settings.theme.waitTimes.background} !important;`"></p>
          </div>
          <AttractionFooter :attraction="this.attraction" :notificationProperties="this.notification.properties" :isFavourite="isFavourite"></AttractionFooter>
        </template>

        <!-- If this is an park-wide notification, then we can show all of the park details and options. -->
        <template v-if="isParkNotification">
          <ParkBackground :park="this.park"></ParkBackground>
          <ParkDetails :park="this.park" :destinationName="destinationName"></ParkDetails>
          <div class="notification__details" v-if="this.notification" :style="`background: ${settings.theme.waitTimes.text} !important;`">
            <p v-html="getNotificationMessage(notification)" :style="`color: ${settings.theme.waitTimes.background} !important;`"></p>
          </div>
          <ParkFooter :notificationProperties="this.notification.properties" :isFavourite="isFavourite" :park="this.park"></ParkFooter>
        </template>
      </IonLabel>
    </IonItem>
    <IonItemOptions @ionSwipe="deleteNotification(notification.properties)">
      <IonItemOption color="danger" @click.stop="deleteNotification(notification.properties)" expandable>Delete</IonItemOption>
    </IonItemOptions>
  </IonItemSliding>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import {mapGetters, mapState} from "vuex";
import {
  ActionSheetButton,
  actionSheetController,
  IonItem,
  IonItemOption, IonItemOptions,
  IonItemSliding,
  IonLabel
} from "@ionic/vue";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import NotificationHoldingArea from "@/models/store/NotificationHoldingArea";
import Attraction from "@/models/api/Attraction";
import Park from "@/models/api/Park";
import {AttractionStatus} from "@/models/enums/AttractionStatus";
import Notification from "@/models/api/Notification";
import {NotificationCriteria} from "@/models/enums/NotificationCriteria";
import {hideBannerAdvertisement, resumeBannerAdvertisement} from "@/handlers/advertisements.handler";
import AttractionDetails from "@/components/attraction/AttractionDetails.vue";
import AttractionFooter from "@/components/attraction/AttractionFooter.vue";
import NotificationProperties from "@/models/api/NotificationProperties";
import AttractionFeature from "@/components/FeatureComponent.vue";
import {NotificationType} from "@/models/enums/NotificationType";
import ParkDetails from "@/components/park/ParkDetails.vue";
import ParkBackground from "@/components/park/ParkBackground.vue";
import ParkFooter from "@/components/park/ParkFooter.vue";

export default defineComponent({
  name: "NotificationComponent",
  components: {
    AttractionDetails,
    AttractionFooter,
    IonItem,
    IonItemOptions,
    IonItemOption,
    IonItemSliding,
    IonLabel,
    ParkDetails,
    ParkBackground,
    ParkFooter
  },
  computed: {
    ...mapState(['notifications', 'isApp', 'settings']),
    ...mapGetters(['favourites', 'notificationAttractionIds', 'notificationParkIds']),
    type() {
      if(this.isAttractionNotification) {
        return 'Attraction';
      }

      if(this.isParkNotification) {
        return 'Park';
      }

      return 'Attraction';
    },
    isAttractionNotification() {
      return this.notification.properties?.typeId === 1;
    },
    isParkNotification() {
      return this.notification.properties?.typeId === 2;
    },
    isFavourite() {
      if(this.isAttractionNotification) {
        return this.favourites.includes(this.attraction?.attractionId);
      }

      if(this.isParkNotification) {
        return this.favourites.includes(this.park?.parkId);
      }

      return false;
    },
    attraction() {
      return this.notification.attraction;
    },
    park() {
      return this.notification.park;
    },
  },
  props: {
    notification: {
      type: Notification,
      required: true,
      default: null
    },
    destinationName: {
      type: String,
      required: true,
      default: ''
    }
  },
  data() {
    return {
      options: false,
      requestLoading: false
    }
  },
  methods: {
    deleteNotification(properties: NotificationProperties) {
      this.$store.dispatch('deleteNotification', properties.itemId);
    },

    showNotificationOptions(attraction: Attraction, park: Park) {
      // if(this.notificationAttractionIds.includes(attraction.attractionId)) {
        this.configureNotification(attraction, park);
      // }
    },

    configureNotification(attraction: Attraction, park: Park) {
      let buttons: Array<ActionSheetButton> = [];


      // if(!this.notificationAttractionIds.includes(attraction.attractionId)) {
      //   buttons.push({
      //     text: 'Create Wait Time Notification',
      //     data: {
      //       action: 'share',
      //     },
      //     handler: () => {
      //       this.$store.commit('setNotificationHoldingArea', new NotificationHoldingArea({ attraction, park, type: this.notification.properties.typeId }));
      //
      //       this.$router.push({
      //         name: 'notificationsCreate',
      //         params: {
      //           transition: 'slide-right'
      //         }
      //       })
      //
      //       resumeBannerAdvertisement(this.settings.parkPalPlus);
      //     }
      //   })
      // }else{
        let header = attraction.name;

        if(this.notification.properties.typeId == NotificationType.Park) {
          header = park.name;
        }

        buttons.push({
          text: 'Edit Wait Time Notification',
          data: {
            action: 'share',
          },
          handler: () => {
            this.$store.commit('setNotificationHoldingArea', new NotificationHoldingArea({ attraction, park, type: this.notification.properties.typeId }));

            this.$router.push({
              name: 'notificationsEdit',
              params: {
                notificationId: this.notifications.find((a: Notification) => a.properties?.attractionId == attraction.attractionId).properties.itemId,
                transition: 'slide-right'
              }
            })

            this.$store.dispatch('setModalOpen', false);
            resumeBannerAdvertisement(this.settings.parkPalPlus);
          }
        })
      // }

      buttons.push({
        text: 'Cancel',
        role: 'cancel',
        data: {
          action: 'cancel',
        },
        handler: () => {
          this.$store.dispatch('setModalOpen', false);
          resumeBannerAdvertisement(this.settings.parkPalPlus);
        }
      });

      const presentActionSheet = async () => {
        const actionSheet = await actionSheetController.create({
          header,
          buttons: buttons
        });

        this.$store.dispatch('setModalOpen', true);
        hideBannerAdvertisement();

        await actionSheet.present();

      }

      presentActionSheet();
    },
    // Format our notification message.
    getNotificationMessage(notification: Notification) {
      if(notification.properties?.waitTime) {
        let message = `Notification activated if `;

        if(notification.properties.typeId == NotificationType.Attraction) {
          message += `wait time is <strong>`;
        }else{
          message += `<strong><u>any ride in the park</u></strong> has a wait time that is <strong>`;
        }

        switch (notification.properties?.criteriaType) {
          case NotificationCriteria.LessThan:
            message += 'less than '
            break;
          case NotificationCriteria.MoreThan:
            message += 'more than '
            break;
          case NotificationCriteria.EqualTo:
            message += 'equal to '
            break;
        }

        message += notification.properties?.waitTime + ` minutes</strong>.`;

        if(!notification.properties?.enabled) {
          if(notification.properties.typeId == NotificationType.Attraction) {
            if ((this.notificationAttractionIds.includes(notification.attraction?.attractionId))) {
              message += " This notification is currently <strong>disabled</strong>."
            }
          }

          if(notification.properties.typeId == NotificationType.Park) {
            if ((this.notificationParkIds.includes(notification.park?.parkId))) {
              message += " This notification is currently <strong>disabled</strong>."
            }
          }
        }

        return message;
      }
    },
  },
});
</script>

<style lang="scss" scoped>
.swiper-gestures {
  position: absolute; top: 0;
  right: 0;
  bottom: 0;
  left: 0;
}

.notification {
  width: auto;
  list-style: none;
  margin: 0 16px 16px;
  min-height: 132px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  position: relative;
  border-radius: 20px;
  overflow: hidden;
  flex-direction: column;

  &.notification--favourite {
    border-style: solid;
    border-width: 4px;
    border-color: #FF4141;
  }

  &.notification--park {
    min-height: unset;

    ion-item {
      width: 100%;
    }

    .notification__details {
      position: relative;
      z-index: 7;
    }

  }

  .notification__type {
    position: absolute;
    top: 0;
    right: 0;
    font-size: 12px;
    background: #9b9292;
    z-index: 26;
    border-radius: 0 16px;
    padding: 8px 18px;
    margin: 0;
    line-height: unset;
    text-align: center;
  }

  ion-item {
    --padding-start: 0;
    --padding-end: 0;
    --inner-padding-end: 0;
    --ion-safe-area-right: 0;
    overflow: hidden;

    ion-label {
      margin: 0;
      width: 100%;
      height: 100%;
      background: transparent;
      overflow: hidden;
    }
  }

  ion-item-options {
    border-width: 0;
  }

  .notification__details {
    padding: 8px 10px;
    white-space: normal;

    p {
      margin: 0;
      text-align: left;
      font-size: 14px;
    }
  }

  &.attraction--favourite {
    border-style: solid;
    border-width: 4px;
    border-color: #FF4141;

    .feature--favourite {
      color: #FF4141;
    }
  }

  .feature--has-notification {
    color: #ffc04e;
  }

  .notification-options {
    width: 0;
    height: 100%;
    position: absolute;
    top: 0;
    background: #f53d3d;
    right: 0;
    z-index: 10;
    transition: width .25s;
    margin: 0;
    padding: 0;
    list-style: none;
    display: flex;
    align-items: center;
    justify-content: stretch;
    overflow: hidden;

    li {
      margin: 0;
      padding: 0;
      list-style: none;
      height: 100%;
      width: 100%;
      display: flex;
      align-items: center;
      justify-content: center;
    }
  }

  &.attraction--options {
    .notification-options {
      width: 25%;
    }
  }

  &.attraction--notification {
    //height: 230px;

    .attraction__details {
      flex-direction: column;
      padding: 0 10px;
    }

    .attraction-footer {
      padding: unset;
    }
  }

  .attraction__background {
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    z-index: -1;
    background-size: cover !important;
    background-position: center !important;
  }

  p {
    position: relative;
    z-index: 4;
    font-size: 28px;
    font-weight: 400;
  }

  .attraction__details {
    //position: absolute;
    //bottom: 10px;
    align-self: end;
    display: flex;
    align-items: center;
    width: 100%;
    justify-content: space-between;

    .attraction__wait, .attraction-footer {
      text-align: left;

      h1 {
        margin: 0 0 2px;
      }

      p {
        margin: 4px 0 0;
      }
    }

    .attraction-footer {
      margin: 0 5px;
      width: 100%;

      h1 {
        font-size: 22px;
      }

      p {
        font-size: 16px;
        //color: #6E6E6E;
      }

      .attraction-footer__information {
        display: flex;
        justify-content: space-between;
      }
    }

    .attraction__wait {
      text-align: right;

      h1 {
        font-size: 13px;
      }

      p {
        font-size: 17px;
      }
    }
  }
}

.banners {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  width: 100%;

  .banner {
    position: relative;
    font-size: 10px;
    font-weight: 500;
    padding: 5px 10px;
    margin: 10px 2px 0;
    border-radius: 16px;
    color: #FFF;

    &.banner--low-wait {
      background-color: #1CA70A;
    }

    &.banner--thrill-ride {
      background-color: #E35313;
    }

    &.banner--tame-ride {
      background-color: #2e7198;
    }
  }
}

.features {
  display: flex;
  list-style: none;
  padding: 0;
  margin: 0;
  position: absolute;
  bottom: 10px;
  right: 10px;
  z-index: 3;

  .feature {
    margin: 0 0 0 5px;

    .feature__icon {
      font-size: 20px;
    }
  }
}
</style>