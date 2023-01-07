<template>
  <div class="park-footer" :class="{ 'park-footer--favourite': isFavourite }">
    <ul class="features">
      <FeatureComponent class="feature--delete" icon="trash" @click.stop="deleteNotification(notificationProperties)" v-if="this.notificationProperties && !$route.fullPath.includes('edit') && !$route.fullPath.includes('create') && this.$route.fullPath.includes('notifications')"></FeatureComponent>
      <FeatureComponent class="feature--enabled" :icon="enabledIcon" @click.stop="toggleNotificationEnabled(notificationProperties.itemId)" v-if="this.notificationProperties && !$route.fullPath.includes('edit') && !$route.fullPath.includes('create') && this.$route.fullPath.includes('notifications')"></FeatureComponent>
      <FeatureComponent class="feature--favourite" icon="heart" @click.stop="favouritePark(park.parkId)"></FeatureComponent>
      <slot></slot>
    </ul>
  </div>
</template>

<script lang="ts">
import {defineComponent} from "vue";
import {mapGetters} from "vuex";
import NotificationProperties from "@/models/api/NotificationProperties";
import FeatureComponent from "@/components/FeatureComponent.vue";
import Park from "@/models/api/Park";

export default defineComponent({
  name: "ParkFooter",
  props: {
    park: {
      type: Park,
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
    FeatureComponent
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
    toggleNotificationEnabled(notificationId: number) {
      // this.requestLoading = true;
      if(this.notificationProperties?.enabled) {
        this.$store.dispatch('setNotificationDisabled', notificationId).then(() => {
          setTimeout(() => {
            // this.requestLoading = false;
          }, 400)
        });
      }else{
        this.$store.dispatch('setNotificationEnabled', notificationId).then(() => {
          setTimeout(() => {
            // this.requestLoading = false;
          }, 400)
        });
      }
    },

    favouritePark(id: string) {
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
.park-footer {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  padding: 10px;
  z-index: 4;
  position: relative;

  &.park-footer--favourite {
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