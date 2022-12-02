<template>
  <li class="attraction" :class="{ 'attraction--favourite': isFavourite, 'attraction--notification': this.notificationProperties, 'attraction--options': this.options }" @click="showNotificationOptions(attraction, park)" :style="`background: ${settings.theme.waitTimes.background} !important; color: ${settings.theme.waitTimes.text} !important;`">
    <AttractionDetails class="attraction-details--wait-time" :attraction="attraction"></AttractionDetails>
    <AttractionFooter :attraction="this.attraction" :notificationProperties="this.notificationProperties" :isFavourite="isFavourite">
                  <AttractionFeature icon="clock" :class="`feature--notification ${notificationAttractionIds.includes(attraction.attractionId) ? 'feature--has-notification': ''}`" @click.stop="configureNotification(attraction, park)" v-if="(settings.parkPalPlus || (!settings.parkPalPlus && notificationAttractionIds.length < 3)) && !$route.fullPath.includes('edit') && !$route.fullPath.includes('create') && !this.$route.fullPath.includes('notifications')"></AttractionFeature>
    </AttractionFooter>

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
import Notification from "@/models/api/Notification";
import {NotificationCriteria} from "@/models/enums/NotificationCriteria";
import {hideBannerAdvertisement, resumeBannerAdvertisement} from "@/handlers/advertisements.handler";
import AttractionDetails from "@/components/attraction/AttractionDetails.vue";
import AttractionFooter from "@/components/attraction/AttractionFooter.vue";
import NotificationProperties from "@/models/api/NotificationProperties";
import AttractionFeature from "@/components/FeatureComponent.vue";
import {configureNotification} from "@/handlers/modals.handler";

export default defineComponent({
  name: "AttractionComponent",
  props: {
    attraction: {
      type: Attraction,
      default: null
    },
    park: {
      type: Park,
      default: null
    },
    destinationId: {
      type: String,
      default: ''
    }
  },
  components: {
    AttractionDetails,
    AttractionFeature,
    AttractionFooter
  },
  computed: {
    ...mapState(['notifications', 'isApp', 'settings']),
    ...mapGetters(['favourites', 'notificationAttractionIds']),
    isFavourite() {
        return this.favourites.includes(this.attraction?.attractionId);
    },
    notificationProperties() {
      const notification = this.notifications.find((a: Notification) => a.properties?.attractionId == this.attraction?.attractionId);

      if(notification) {
        return notification.properties;
      }

      return null;
    }
  },
  data() {
    return {
      attractionStatus: AttractionStatus,
      options: false,
      requestLoading: false
    }
  },
  methods: {
    deleteNotification(attraction: Attraction) {
      this.$store.dispatch('deleteNotification', this.notifications.filter((a: Notification) => a.properties?.attractionId == attraction.attractionId)[0].timer.attractionTimerId);
      resumeBannerAdvertisement(this.settings.parkPalPlus);
    },
    toggleOptions(direction: string) {
      if(this.notificationProperties) {

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
      if(this.notificationAttractionIds.includes(attraction.attractionId)) {
        this.configureNotification(attraction, park);
      }
    },

    configureNotification(attraction: Attraction, park: Park) {
      configureNotification(attraction, park, this.notificationAttractionIds, this.destinationId, this.notifications);
    },
    getWaitTime(waitTime: number) {
      if(waitTime) {
        let returnMessage = waitTime + ' minute';

        if(waitTime > 1) {
          returnMessage += 's';
        }

        return returnMessage;
      }

      return 'Walk On'
    },
    // Format our notification message.
    getNotificationMessage(notification: Notification) {
      if(notification.properties?.waitTime) {
        let message = `NotificationProperties activated if wait time is <strong>`;

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
          if((this.notificationAttractionIds.includes(notification.attraction?.attractionId)) && !this.$route.fullPath.includes('edit') && !this.$route.fullPath.includes('create') && this.$route.fullPath.includes('notifications')) {
            message += " This notification is currently <strong>disabled</strong>."
          }
        }

        return message;
      }
    },
    toggleNotificationEnabled(attractionTimerId: number) {
      this.requestLoading = true;
      if(this.notificationProperties.enabled) {
        this.$store.dispatch('setNotificationDisabled', attractionTimerId).then(() => {
          setTimeout(() => {
            this.requestLoading = false;
          }, 400)
        });
      }else{
        this.$store.dispatch('setNotificationEnabled', attractionTimerId).then(() => {
          setTimeout(() => {
            this.requestLoading = false;
          }, 400)
        });
      }
    }
  },
});
</script>

<style lang="scss" scoped>
.attraction {
  list-style: none;
  margin: 16px 16px;
  position: relative;
  border-radius: 16px;
  overflow: hidden;
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
}

</style>