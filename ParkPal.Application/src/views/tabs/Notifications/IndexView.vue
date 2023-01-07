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
        <template v-if="notifications.length">
          <IonSearchbar placeholder="Search" debounce="400" @ionChange="searchInNotifications" @keyup.enter="dismissKeyboard" v-model="waitTimeSearch" :style="`--background: ${settings.theme.searchBoxBackground}; --color: ${settings.theme.searchBoxText}; --icon-color: ${settings.theme.searchBoxIcons}; --clear-button-color: ${settings.theme.searchBoxIcons};`"></IonSearchbar>
          <ul class="attractions" v-if="!waitTimeSearch.length">
                <NotificationComponent v-for="notification in notifications" :key="notification.properties.itemId" :notification="notification" :destinationName="getDestinationName(notification.properties.parkId)"></NotificationComponent>
          </ul>
          <ul class="attractions" v-if="waitTimeSearch.length">
            <NotificationComponent v-for="notification in searchNotifications" :key="notification.properties.itemId" :notification="notification" :destinationName="getDestinationName(notification.properties.parkId)"></NotificationComponent>
          </ul>
          <div class="no-notifications" v-if="waitTimeSearch.length && !searchNotifications.length">
            <div class="no-notifications__image">
              <img src="@/assets/no-notifications.svg">
            </div>
            <p :style="'color: ' + settings.theme.text + ' !important;'">There are no notifications that match your search criteria, please change your search term and try again.</p>
            <div class="filter-button">
              <IonButton expand="full" @click="waitTimeSearch = ''" color="transparent" :style="`color: ${settings.theme.resetButtonText} !important; background: ${settings.theme.resetButtonBackground} !important;`">
                RESET SEARCH
              </IonButton>
            </div>
          </div>
        </template>
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
  margin: 0;
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

.filter-button {
  width: 100%;

  ion-button {
    margin: 0;
    font-weight: 300;
    font-size: 14px;
  }
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
    IonRefresherContent,
    IonSearchbar
} from "@ionic/vue";
import {FontAwesomeIcon} from "@fortawesome/vue-fontawesome";
import Loader from "../../../components/Loader.vue";
import AlertComponent from "@/components/Alert.vue";
import ConnectionError from "@/components/ConnectionError.vue";
import Notification from "@/models/api/Notification";
import {Keyboard} from "@capacitor/keyboard";
import NotificationComponent from "@/components/Notification.vue";
import {themeparkService} from "@/services/themepark.service";
import {AxiosResponse} from "axios";
import Destination from "@/models/api/Destination";
import {App} from "@capacitor/app";

export default defineComponent({
  name: "NotificationsView",
  components: {
    NotificationComponent,
    AlertComponent,
    IonButtons,
    IonButton,
    IonPage,
    IonContent,
    IonHeader,
    IonToolbar,
    IonTitle,
    FontAwesomeIcon,
    Loader,
    ConnectionError,
    IonRefresher,
    IonRefresherContent,
    IonSearchbar
  },
  computed: {
    ...mapState(['notifications', 'filters', 'settings', 'serverError']),
    ...mapGetters(['favourites', 'notificationIds'])
  },
  data(): { loading: boolean, waitTimeSearch: string, searchNotifications: Array<Notification>, destinations: Array<Destination>} {
    return {
      loading: true,
      waitTimeSearch: '',
      searchNotifications: [],
      destinations: []
    }
  },
  watch: {
    notifications() {
      this.loading = false;
    }
  },
  beforeMount() {
    this.getDestinations();
    // We need to pull in all the clients notifications, if they have any.
    this.getAllNotifications(null);

    App.addListener('resume',() => {
      if(this.$route.name == 'notifications') {
        this.loading = true;
        this.getDestinations();
        // We need to pull in all the clients notifications, if they have any.
        this.getAllNotifications(null);
      }
    })
  },
  methods: {
    getDestinations() {
      this.$store.dispatch('setServerError', false);

      // Get all the destinations first and mark them as hidden.
      themeparkService.getDestinations().then((response: AxiosResponse<Array<Destination>>) => {
        response.data.forEach(destination => {
            let transformedDestination = new Destination(destination);

            this.destinations.push(transformedDestination);
        });

      }).catch(() => {
        this.$store.dispatch('setServerError', true);
      })
    },
    getDestinationName(parkId: string) {
      let destination = this.destinations.find(a => a.parks.find(a => a.parkId == parkId));

      if(destination) {
        return destination.name;
      }

      return '';
    },
    dismissKeyboard() {
      Keyboard.hide();
    },
    getAllNotifications(event: RefresherCustomEvent | null) {
      this.$store.dispatch('getAllNotifications', {
        filters: this.filters.notificationsFilter,
        favouriteIds: this.favourites
      });
      event?.target?.complete();
    },
    filterNotifications() {
      this.$router.push({
        name: 'notificationFilters',
        params: {
          transition: 'slide-right'
        }
      })
    },
    searchInNotifications() {
      this.searchNotifications = this.notifications.filter((a: Notification) => JSON.stringify(a).toLowerCase().includes(this.waitTimeSearch.toLowerCase()));
    },
  }
})
</script>