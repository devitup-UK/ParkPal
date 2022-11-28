<template>
  <li class="attraction" :class="{ 'attraction--favourite': this.favourites.includes(attraction.attractionId), 'attraction--notification': this.notification, 'attraction--options': this.options }" @click="showNotificationOptions(attraction, park)" :style="`background: ${settings.theme.waitTimes.background} !important; color: ${settings.theme.waitTimes.text} !important;`">
    <div class="swiper-gestures" v-touch:swipe="toggleOptions" v-if="this.notification && (notificationIds.includes(attraction.attractionId)) && !$route.fullPath.includes('edit') && !$route.fullPath.includes('create') && this.$route.fullPath.includes('notifications')"></div>
    <ul class="features">
      <li class="feature feature--notification" :class="{ 'feature--has-notification': this.notificationIds.includes(attraction.attractionId) }" @click.stop="configureNotification(attraction, park)" v-if="(settings.parkPalPlus || (!settings.parkPalPlus && notificationIds.length < 3) || notificationIds.includes(attraction.attractionId)) && !$route.fullPath.includes('edit') && !$route.fullPath.includes('create') && !this.$route.fullPath.includes('notifications')">
        <FontAwesomeIcon icon="clock" class="feature__icon" fixed-width></FontAwesomeIcon>
      </li>
      <li class="feature feature--enabled" @click.stop="toggleNotificationEnabled(notification.timer.attractionTimerId)" v-if="(notificationIds.includes(attraction.attractionId)) && !$route.fullPath.includes('edit') && !$route.fullPath.includes('create') && this.$route.fullPath.includes('notifications')">
        <FontAwesomeIcon :icon="this.notification.timer.enabled ? 'bell' : 'bell-slash'" class="feature__icon" fixed-width></FontAwesomeIcon>
      </li>
      <li class="feature feature--favourite" @click.stop="favourite(attraction.attractionId)">
        <FontAwesomeIcon icon="heart" class="feature__icon" fixed-width></FontAwesomeIcon>
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
      <div class="attraction__notification-details" v-if="this.notification" :style="`background: ${settings.theme.waitTimes.text} !important;`">
        <p v-html="getNotificationMessage(notification)" :style="`color: ${settings.theme.waitTimes.background} !important;`"></p>
      </div>
    </div>
    <ul class="banners">
      <li class="banner banner--low-wait" v-if="attraction.waitTime <= 30 && attraction.waitTime != null">
        <span>Low Wait Time</span>
      </li>
      <li class="banner banner--thrill-ride" v-if="attraction.thrill">
        <span>Thrill Ride</span>
      </li>
      <li class="banner banner--tame-ride" v-if="!attraction.thrill">
        <span>Tame Ride</span>
      </li>
    </ul>
    <ul class="notification-options">
      <li @click.stop="deleteNotification(attraction)">
        <FontAwesomeIcon icon="trash"></FontAwesomeIcon>
      </li>
    </ul>
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
import {hideBannerAdvertisement, resumeBannerAdvertisement} from "@/handlers/advertisements.handler";

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
      attractionStatus: AttractionStatus,
      options: false
    }
  },
  methods: {
    deleteNotification(attraction: Attraction) {
      this.$store.dispatch('deleteNotification', this.notifications.filter((a: TimerWithAttraction) => a.timer?.attractionId == attraction.attractionId)[0].timer.attractionTimerId);
      resumeBannerAdvertisement(this.settings.parkPalPlus);
    },
    toggleOptions(direction: string) {
      if(this.notification) {
        this.options = direction == 'left';
      }
    },

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
                destinationId: this.destinationId,
                transition: 'slide-right'
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
                attractionTimerId: this.notifications.filter((a: TimerWithAttraction) => a.timer?.attractionId == attraction.attractionId)[0].timer.attractionTimerId,
                transition: 'slide-right'
              }
            })

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
      if(notification.timer?.waitTime) {
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

        if(!notification.timer?.enabled) {
          if((this.notificationIds.includes(notification.attraction?.attractionId)) && !this.$route.fullPath.includes('edit') && !this.$route.fullPath.includes('create') && this.$route.fullPath.includes('notifications')) {
            message += " This notification is currently <strong>disabled</strong>."
          }
        }

        return message;
      }
    },
    toggleNotificationEnabled(attractionTimerId: number) {
      if(this.notification.timer.enabled) {
        this.$store.dispatch('setNotificationDisabled', attractionTimerId);
      }else{
        this.$store.dispatch('setNotificationEnabled', attractionTimerId);
      }
    }
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

.attraction {
  list-style: none;
  margin: 16px 16px;
  padding: 10px;
  min-height: 132px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  //color: #FFF;
  position: relative;
  border-radius: 16px;
  overflow: hidden;
  //background-image: linear-gradient(transparent, white 100%);
  flex-direction: column;

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

    .attraction__notification-details {
      margin: 15px -20px 5px;
      background: #8a8a8a;
      padding: 8px 10px;

      p {
        margin: 0;
        text-align: left;
        font-size: 14px;
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