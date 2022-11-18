<template>
  <IonPage>
    <IonHeader :style="'background: ' + settings.theme.header.background + ' !important;'">
      <IonToolbar color="transparent" :style="'background: ' + settings.theme.header.background + ' !important;'">
        <IonTitle :style="'background: ' + settings.theme.header.background + ' !important; border-width: 0 0px 2px; border-style: solid;border-color: ' + settings.theme.header.border + ' !important; color: '+ settings.theme.header.text + ' !important;'">Notifications</IonTitle>
        <IonButtons slot="end">
          <IonButton @click="filterNotifications">
            <FontAwesomeIcon icon="filter" fixed-width :color="settings.theme.header.icons"></FontAwesomeIcon>
          </IonButton>
        </IonButtons>
      </IonToolbar>
    </IonHeader>
    <IonContent :style="'background: ' + settings.theme.background + ' !important;'">
      <IonRefresher slot="fixed" @ionRefresh="getAllNotifications($event)">
        <IonRefresherContent pullingText="Refresh Wait Times" refreshingText="Fetching Wait Times..."></IonRefresherContent>
      </IonRefresher>
      <ConnectionError v-if="serverError" @retry="getAllNotifications(null)"></ConnectionError>
      <Loader v-if="loading && !serverError">Fetching Notifications...</Loader>
      <template v-else>
        <AlertComponent v-if="!settings.parkPalPlus">You can only have a maximum of 3 notifications. Subscribe to ParkPal+ to set an unlimited amount of notifications.</AlertComponent>
        <ul class="attractions" v-if="notifications.length">
          <AttractionComponent v-for="notification in notifications" :key="notification.timer.attractionTimerId" :attraction="notification.attraction" :park="notification.park" :notification="notification"></AttractionComponent>
        </ul>
        <div class="no-notifications" v-else>
          <div class="no-notifications__image">
            <img src="@/assets/no-notifications.svg">
          </div>
          <p :style="'color: ' + settings.theme.text + ' !important;'">There are no notifications that match your filter criteria or you currently have no notifications setup, please change your filters or head to the <strong><u>Destinations</u></strong> tab to find attractions.</p>
        </div>
      </template>
    </IonContent>
  </IonPage>
</template>

<style lang="scss" scoped>
.attractions {
  list-style: none;
  padding: 0;
  margin: 16px 0 0;
}

.no-notifications {
  display: flex;
  height: 100%;
  align-items: center;
  justify-content: center;
  flex-direction: column;

  p {
    font-size: 14px;
    margin: 10px 20px 14px;
  }
}

.refresher-pulling-text, .refresher-refreshing-text {
  text-transform: uppercase !important;
  font-size: 12px !important;
  color: #3f3f3f;
}
</style>

<script lang="ts">
import { defineComponent } from "vue";
import AttractionComponent from "../../../components/Attraction.vue";
import {mapGetters, mapState} from "vuex";
import {
  IonPage,
  IonContent,
  IonHeader,
  IonToolbar,
  IonTitle,
  IonButton,
  IonButtons,
  RefresherCustomEvent,
    IonRefresher,
    IonRefresherContent
} from "@ionic/vue";
import {FontAwesomeIcon} from "@fortawesome/vue-fontawesome";
import Loader from "../../../components/Loader.vue";
import AlertComponent from "@/components/Alert.vue";
import ConnectionError from "@/components/ConnectionError.vue";

export default defineComponent({
  name: "NotificationsView",
  components: {
    AlertComponent,
    IonButtons,
    IonButton,
    IonPage,
    IonContent,
    IonHeader,
    IonToolbar,
    IonTitle,
    AttractionComponent,
    FontAwesomeIcon,
    Loader,
    ConnectionError,
    IonRefresher,
    IonRefresherContent
  },
  computed: {
    ...mapState(['notifications', 'filters', 'settings', 'serverError']),
    ...mapGetters(['favourites', 'notificationIds'])
  },
  data() {
    return {
      loading: true
    }
  },
  watch: {
    notifications() {
      this.loading = false;
    }
  },
  beforeMount() {
    // We need to pull in all the clients notifications, if they have any.
    this.getAllNotifications(null);
  },
  methods: {
    getAllNotifications(event: RefresherCustomEvent | null) {
      this.$store.dispatch('getAllNotifications', {
        filters: this.filters.notificationsFilter,
        favouriteAttractionIds: this.favourites
      });
      event?.target?.complete();
    },
    filterNotifications() {
      this.$router.push({
        name: 'notificationFilters'
      })
    },
  }
})
</script>