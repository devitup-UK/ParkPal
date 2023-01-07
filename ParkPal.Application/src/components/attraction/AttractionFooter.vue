<template>
  <div class="attraction-footer" :class="{ 'attraction-footer--favourite': isFavourite }">
      <ul class="banners">
        <AttractionBanner class="banner--low-wait" v-if="attraction.waitTime <= 30 && attraction.waitTime != null">Low Wait Time</AttractionBanner>
        <AttractionBanner class="banner--thrill-ride" v-if="attraction.thrill">Thrill Ride</AttractionBanner>
        <AttractionBanner class="banner--tame-ride" v-if="!attraction.thrill">Tame Ride</AttractionBanner>
      </ul>
      <ul class="features">
        <FeatureComponent class="feature--delete" icon="trash" @click.stop="deleteNotification(notificationProperties)" v-if="this.notificationProperties && !$route.fullPath.includes('edit') && !$route.fullPath.includes('create') && this.$route.fullPath.includes('notifications')"></FeatureComponent>
        <FeatureComponent class="feature--enabled" :icon="enabledIcon" @click.stop="toggleNotificationEnabled(notificationProperties.itemId)" v-if="this.notificationProperties && !$route.fullPath.includes('edit') && !$route.fullPath.includes('create') && this.$route.fullPath.includes('notifications')"></FeatureComponent>
        <FeatureComponent class="feature--favourite" icon="heart" @click.stop="favouriteAttraction(attraction.attractionId)"></FeatureComponent>

        <slot></slot>
      </ul>
  </div>
</template>

<script lang="ts">
import {defineComponent} from "vue";
import Attraction from "@/models/api/Attraction";
import {mapGetters} from "vuex";
import NotificationProperties from "@/models/api/NotificationProperties";
import AttractionBanner from "@/components/attraction/AttractionBanner.vue";
import FeatureComponent from "@/components/FeatureComponent.vue";

export default defineComponent({
  name: "AttractionFooter",
  props: {
    attraction: {
      type: Attraction,
      required: true,
      default: null
    },
    notificationProperties: {
      type: NotificationProperties,
      default: undefined
    },
    isFavourite: {
      type: Boolean,
      default: false
    }
  },
  components: {
    FeatureComponent,
    AttractionBanner
  },
  computed: {
    ...mapGetters(['notificationAttractionIds', 'favourites', 'settings']),
    enabledIcon(): string {
      if(this.notificationProperties) {
        if (this.notificationProperties.enabled) {
          return 'bell';
        } else {
          return 'bell-slash';
        }
      }

      return '';
    }
  },
  methods: {
    deleteNotification(properties: NotificationProperties) {
      this.$store.dispatch('deleteNotification', properties.itemId);
    },
    toggleNotificationEnabled(attractionTimerId: number) {
      // this.requestLoading = true;
      if(this.notificationProperties?.enabled) {
        this.$store.dispatch('setNotificationDisabled', attractionTimerId).then(() => {
          setTimeout(() => {
            // this.requestLoading = false;
          }, 400)
        });
      }else{
        this.$store.dispatch('setNotificationEnabled', attractionTimerId).then(() => {
          setTimeout(() => {
            // this.requestLoading = false;
          }, 400)
        });
      }
    },

    favouriteAttraction(id: string) {
      if (this.favourites.includes(id)) {
        this.$store.dispatch('removeFavourite', id);
      } else {
        this.$store.dispatch('addFavourite', id);
      }
    },

  }
})
</script>

<style lang="scss" scoped>
.attraction-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 10px;

  &.attraction-footer--favourite {
    .features {
      .feature {
        &.feature--favourite {
          color: #FF4141;
        }
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
}

.features {
  display: flex;
  list-style: none;
  padding: 0;
  margin: 0;

  .feature {
    margin: 0 0 0 5px;

    .feature__icon {
      font-size: 20px;
    }
  }
}
</style>