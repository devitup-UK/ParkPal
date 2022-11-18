<template>
  <li class="attraction" :class="{ 'attraction--favourite': this.favourites.includes(attraction.attractionId), 'attraction--notification': this.notification }" @click="showNotificationOptions(attraction, park)">
    <div class="attraction__background" :style="'background: url(/img/' + attraction.attractionId + '.jpeg)'"></div>
    <ul class="features">
      <li class="feature feature--favourite ion-margin-bottom" @click.stop="favourite(attraction.attractionId)">
        <FontAwesomeIcon icon="heart" size="2x" fixed-width></FontAwesomeIcon>
      </li>
      <li class="feature feature--notification" :class="{ 'feature--has-notification': this.notificationIds.includes(attraction.attractionId) }" @click.stop="configureNotification(attraction, park)" v-if="settings.parkPalPlus || (!settings.parkPalPlus && notificationIds.length < 3) || notificationIds.includes(attraction.attractionId)">
        <FontAwesomeIcon icon="clock" size="2x" fixed-width></FontAwesomeIcon>
      </li>
    </ul>
    <ul class="banners">
      <li class="banner banner--low-wait" v-if="attraction.waitTime <= 30 && attraction.waitTime != null">
        <span>Low Wait Time</span>
      </li>
      <li class="banner banner--thrill-ride" v-if="attraction.thrill">
        <span>Thrill Ride</span>
      </li>
    </ul>
    <div class="attraction__details">
      <div class="attraction-footer">
        <h1>{{ attraction.name }}</h1>
        <div class="attraction-footer__information">
          <p>{{ attractionStatus[attraction.status] }}</p>
          <p v-if="attraction.waitTime != null">{{ attraction.waitTime }} minutes</p>
        </div>
      </div>
      <div class="attraction__notification-details" v-if="this.notification">
        <p v-html="getNotificationMessage(notification)"></p>
      </div>
    </div>
  </li>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import {mapGetters, mapState} from "vuex";
import {ActionSheetButton, actionSheetController} from "@ionic/vue";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import NotificationHoldingArea from "@/models/store/NotificationHoldingArea";
import Attraction from "@/models/api/Attraction";
import Park from "@/models/api/Park";
import {AttractionStatus} from "@/models/enums/AttractionStatus";
import TimerWithAttraction from "@/models/api/TimerWithAttraction";
import {NotificationCriteria} from "@/models/enums/NotificationCriteria";
import AttractionTimer from "@/models/api/AttractionTimer";
import {AdMob, BannerAdPosition, BannerAdSize} from "@capacitor-community/admob";
import {hideBannerAdvertisement, resumeBannerAdvertisement, showBannerAdvertisement} from "@/events/advertisements.bus";

export default defineComponent({
  name: "AttractionComponent",
  components: {
    FontAwesomeIcon
  },
  computed: {
    ...mapState(['notifications', 'isApp', 'settings']),
    ...mapGetters(['favourites', 'notificationIds'])
  },
  props: ['attraction', 'notification', 'park', 'destinationId'],
  data() {
    return {
      attractionStatus: AttractionStatus
    }
  },
  methods: {
    favourite(id: string) {
      if (this.favourites.includes(id)) {
        this.$store.dispatch('removeFavourite', id);
      } else {
        this.$store.dispatch('addFavourite', id);
      }
    },

    showNotificationOptions(attraction: Attraction, park: Park) {
      if(this.notificationIds.includes(attraction.attractionId)) {
        this.configureNotification(attraction, park);
      }
    },

    configureNotification(attraction: Attraction, park: Park) {
      let buttons: Array<ActionSheetButton> = [];

      if(!this.notificationIds.includes(attraction.attractionId)) {
        buttons.push({
          text: 'Create Wait Time Notification',
          data: {
            action: 'share',
          },
          handler: () => {
            this.$store.commit('setNotificationHoldingArea', new NotificationHoldingArea({ attraction, park }));

            this.$router.push({
              name: 'notificationsCreate',
              params: {
                destinationId: this.destinationId
              }
            })

            resumeBannerAdvertisement(this.settings.parkPalPlus);
          }
        })
      }else{
        buttons.push({
          text: 'Edit Wait Time Notification',
          data: {
            action: 'share',
          },
          handler: () => {
            this.$store.commit('setNotificationHoldingArea', new NotificationHoldingArea({ attraction, park }));

            this.$router.push({
              name: 'notificationsEdit',
              params: {
                attractionTimerId: this.notifications.filter((a: TimerWithAttraction) => a.timer?.attractionId == attraction.attractionId)[0].timer.attractionTimerId
              }
            })

            resumeBannerAdvertisement(this.settings.parkPalPlus);
          }
        })

        buttons.push({
          text: 'Delete Wait Time Notification',
          role: 'destructive',
          data: {
            action: 'delete',
          },
          handler: () => {
            // This will delete the notification.
            this.$store.dispatch('deleteNotification', this.notifications.filter((a: TimerWithAttraction) => a.timer?.attractionId == attraction.attractionId)[0].timer.attractionTimerId);
            resumeBannerAdvertisement(this.settings.parkPalPlus);
          }
        })
      }

      buttons.push({
        text: 'Cancel',
        role: 'cancel',
        data: {
          action: 'cancel',
        },
        handler: () => {
          resumeBannerAdvertisement(this.settings.parkPalPlus);
        }
      });

      const presentActionSheet = async () => {
        const actionSheet = await actionSheetController.create({
          header: attraction.name,
          subHeader: 'Create a notification for this attraction?',
          buttons: buttons
        });

        hideBannerAdvertisement();

        await actionSheet.present();

      }

      presentActionSheet();
    },
    // Format our notification message.
    getNotificationMessage(notification: TimerWithAttraction) {
      if(notification.attraction?.waitTime) {
        let message = `Notification activated if wait time is <strong>`;

        switch (notification.timer?.criteriaType) {
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

        message += notification.timer?.waitTime + ` minutes</strong>.`;

        return message;
      }
    }
  }
});
</script>

<style lang="scss" scoped>
.attraction {
  list-style: none;
  margin: 16px 16px;
  padding: 0 10px;
  height: 195px;
  display: flex;
  justify-content: center;
  align-items: center;
  color: #FFF;
  position: relative;
  border-radius: 16px;
  overflow: hidden;
  background-image: linear-gradient(transparent, white 100%);

  &:nth-child(1) {
    margin: 0 16px 16px;
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

  &.attraction--notification {
    height: 230px;

    .attraction__details {
      flex-direction: column;
      padding: 0 10px;
    }

    .attraction-footer {
      padding: unset;
    }

    .attraction__notification-details {
      flex-grow: 1;
      margin: 10px 10px 0;
      width: 100%;

      p {
        margin: 0;
        text-align: left;
        font-size: 14px;
        color: #969696;
      }
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
    position: absolute;
    bottom: 10px;
    align-self: end;
    display: flex;
    align-items: center;
    color: #000;
    width: 100%;
    justify-content: space-between;

    .attraction__wait, .attraction-footer {
      text-align: left;

      h1 {
        margin: 0;
      }

      p {
        margin: 4px 0 0;
      }
    }

    .attraction-footer {
      margin: 0 10px;
      width: 100%;

      h1 {
        font-size: 22px;
      }

      p {
        font-size: 16px;
        color: #6E6E6E;
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
  position: absolute;
  top: 0;
  left: 0;
  list-style: none;
  padding: 0;
  margin: 5px 0 0;

  .banner {
    position: relative;
    font-size: 17px;
    font-weight: 500;
    height: 32px;
    display: flex;
    align-items: center;
    padding: 0 10px;
    margin: 5px 0 0;

    &::before, &::after {
      content: "";
      position: absolute;
      top: 0;
      left: 99%;
      width: 16px;
      height: 50%;
    }

    &::after {
      top: 50%;
      transform: scaleY(-1);
    }

    &.banner--low-wait {
      background-color: #1CA70A;
      background-image: linear-gradient(to left top, transparent 50%, #1CA70A 50%);

      &::before, &::after {
        background-image: linear-gradient(to left top, transparent 50%, #1CA70A 50%);
      }
    }

    &.banner--thrill-ride {
      background-color: #E35313;
      background-image: linear-gradient(to left top, transparent 50%, #E35313 50%);

      &::before, &::after {
        background-image: linear-gradient(to left top, transparent 50%, #E35313 50%);
      }
    }
  }
}

.features {
  list-style: none;
  padding: 0;
  margin: 0;
  position: absolute;
  top: 4px;
  right: 10px;
  z-index: 3;

  .feature {
    margin-top: 4px;
  }
}
</style>