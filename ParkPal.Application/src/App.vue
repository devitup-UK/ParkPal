<template>
<!--  <nav>-->
  <IonApp>
    <div class="content" :class="{ 'content--ads': !this.settings.parkPalPlus && this.isApp }">
      <RouterView></RouterView>
    </div>
    <div class="tabs" ref="tabs">
      <IonTabBar :style="'background:' + settings.theme.navigation.background + ' !important; border-color: ' + settings.theme.navigation.border + ' !important;'">
        <IonTabButton tab="destinations" @click="this.$router.push({ name: 'destinations' })" href="/tabs/destinations" :style="'background:' + settings.theme.navigation.background + ' !important; color:' + settings.theme.navigation.text + ' !important;'">
          <div class="active-indicator" v-if="isActiveTab('destinations').active" :style="'background:' + settings.theme.navigation.border + ' !important;'"></div>
          <FontAwesomeIcon icon="map-marker-alt" size="3x" :color="isActiveTab('destinations').icon" fixed-width />
          <IonLabel :style="'color: ' + isActiveTab('destinations').text + ' !important;'">DESTINATIONS</IonLabel>
        </IonTabButton>

        <IonTabButton tab="notifications" @click="this.$router.push({ name: 'notifications' })" href="/tabs/notifications" :style="'background:' + settings.theme.navigation.background + ' !important'">
          <div class="active-indicator" v-if="isActiveTab('notifications').active" :style="'background:' + settings.theme.navigation.border + ' !important;'"></div>
          <FontAwesomeIcon icon="clock" size="3x" :color="isActiveTab('notifications').icon" fixed-width />
          <IonLabel :style="'color: ' + isActiveTab('notifications').text + ' !important;'">NOTIFICATIONS</IonLabel>
        </IonTabButton>

        <IonTabButton tab="settings" @click="this.$router.push({ name: 'settings' })" href="/tabs/settings" :style="'background:' + settings.theme.navigation.background + ' !important'">
          <div class="active-indicator" v-if="isActiveTab('settings').active" :style="'background:' + settings.theme.navigation.border + ' !important;'"></div>
          <FontAwesomeIcon icon="cog" size="3x" :color="isActiveTab('settings').icon" fixed-width />
          <IonLabel :style="'color: ' + isActiveTab('settings').text + ' !important;'">SETTINGS</IonLabel>
        </IonTabButton>
      </IonTabBar>
    </div>
<!--    <Advertisements></Advertisements>-->
  </IonApp>
</template>

<style lang="scss">
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}

 ion-tab-bar {
   border-width: 2px 0 0;
   border-style: solid;
   border-color: var(--ion-color-primary);
   height: 82px;
 }

ion-label {
  margin-top: 6px;
}

.tab-selected {
  position: relative;

  &::before {
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    height: 3px;
    background-color: var(--ion-color-primary);
    content: '';
  }
}

.content {
  position: relative;
  height: 100%;

  &.content--ads {
    margin-bottom: 60px;
  }
}

.active-indicator {
  position: absolute;
  top: 0;
  right: -2px;
  left: -2px;
  height: 4px;
}

ion-content {
  --background: transparent !important;
}
</style>

<script lang="ts">
import {IonApp, IonLabel, IonTabBar, IonTabButton} from '@ionic/vue';
import {PushNotifications} from '@capacitor/push-notifications';
import {ScreenOrientation} from "@awesome-cordova-plugins/screen-orientation";
import {defineComponent} from 'vue';
import {FontAwesomeIcon} from '@fortawesome/vue-fontawesome';
import {mapState} from 'vuex';
import {tokenService} from "@/services/token.service";
import {RouterView} from "vue-router";
// import Advertisements from '@/components/Advertisements.vue';
import {hideBannerAdvertisement, initialiseAdvertisements, showBannerAdvertisement} from '@/events/advertisements.bus';
import {
  requestNotificationPermissions,
  saveSubscriptionToDatabase,
  setupOneSignal
} from "@/handlers/notifications.handler";
import {App} from '@capacitor/app';
import {StatusBar, Style} from "@capacitor/status-bar";
import OneSignal from "onesignal-cordova-plugin";
import store from "@/store";
import {IAPProduct, InAppPurchase2} from "@ionic-native/in-app-purchase-2";
import parkpalplusHandler from "@/handlers/parkpalplus.handler";
import {CapacitorPurchases, Package, PurchaserInfo} from "@capgo/capacitor-purchases";


export default defineComponent({
  name: 'App',
  components: {
    IonApp,
    // IonRouterOutlet,
    // IonTabs,
    IonTabBar,
    IonTabButton,
    IonLabel,
    FontAwesomeIcon,
    RouterView
  },
  computed: {
    ...mapState(['settings', 'isApp', 'notificationsEnabled'])
  },

  methods: {
    generateAndSaveToken() {
      tokenService.generate().then((token: string | undefined) => {
        this.$store.dispatch('setToken', token);
        this.$store.dispatch('reSaveSettings');
      })
    },
    isActiveTab(path: string) {
      if(this.$route.fullPath.includes('/tabs/' + path)) {
        return  {
          icon: this.settings.theme.navigation.activeIcon,
          text: this.settings.theme.navigation.activeText,
          active: true
        }
      }else{
        return  {
          icon: this.settings.theme.navigation.icons,
          text: this.settings.theme.navigation.text,
          active: false
        }
      }
    }
  },

  // We need to check if our local storage has our Settings before, this is where all of our data is stored even after the app has been force closed.
  beforeMount() {

    this.$store.dispatch('configureSettings');


    // First we need to check if we have an API token set in our settings, this would have come from our localStorage if this is a returning user.
    if(!this.settings.apiToken) {
      this.generateAndSaveToken();
    }

    // Setup our OneSignalProperties
    if(this.isApp) {
      ScreenOrientation.lock(ScreenOrientation.ORIENTATIONS.PORTRAIT);
      parkpalplusHandler.setDebugLogLevel();
      parkpalplusHandler.initialisePurchases();

      // Setup OneSignal with all of our details, this could be a first ever launch of our app or a user opening the app after closing it.
      setupOneSignal().then(() => {
        console.log('One Signal has been setup.');
        // OneSignal setup complete and verified.
        requestNotificationPermissions().then(() => {
          console.log('Notifications permissions have been accepted.');
          // We have received word that they have accepted permissions, we will now save the OneSignal subscription to the database.
          saveSubscriptionToDatabase().then(() => {
            console.log('Subscription saved to database.');
            this.$store.dispatch('setNotificationsEnabled', true);
          }).catch(() => {
            console.log('Error saving subscription token to database.');
            this.$store.dispatch('setNotificationsEnabled', false);
          });
        }).catch(() => {
          console.log('Notifications permissions have been declined.');
          this.$store.dispatch('setNotificationsEnabled', false);
        })
      });


      // If the app has been resumed and PushNotifications are no longer granted, we can set the notifications flag to false.
      App.addListener('resume',() => {
        parkpalplusHandler.restorePurchases().then((hasParkPalPlus: boolean) => {
          if(hasParkPalPlus) {
            hideBannerAdvertisement();
          }else{
            showBannerAdvertisement(hasParkPalPlus);
          }
          this.$store.dispatch('setParkPalPlus', hasParkPalPlus);
        });


        console.log('App has been resumed, checking the users permissions in case they have changed.');
        PushNotifications.checkPermissions().then((permissions) => {
          if(permissions.receive == 'granted') {
            console.log('Permissions have been granted.');
            setupOneSignal().then(() => {
              console.log('One Signal has been setup after app has been resumed.');
              saveSubscriptionToDatabase().then(() => {
                console.log('Subscription saved to the database after app has been resumed.');
                this.$store.dispatch('setNotificationsEnabled', true);
              }).catch(() => {
                console.log('Saving subscription to the database has failed after app has been resumed.');
                this.$store.dispatch('setNotificationsEnabled', false);
              });
            });
          }else{
            console.log('Permissions have been disabled or are still disabled from original denial after app has been resumed.');
            this.$store.dispatch('setNotificationsEnabled', false);
          }
        })
      })


      PushNotifications.addListener('pushNotificationReceived', notification => {
        console.log('Push notification received: ', notification);
      });

      // Initialise our advertisements right at the start of the application.
      initialiseAdvertisements();

      // Initialise our store and find out if the Apple ID has already subscribed.
      // parkpalplusHandler.initialisePurchases().then((products: {[p: string]: IAPProduct}) => {
      //   // this.products = products;
      //   this.$store.dispatch('setProducts', products);
      // });

      parkpalplusHandler.getProducts().then((products: Array<Package>) => {
        this.$store.dispatch('setProducts', products);
      });

      parkpalplusHandler.restorePurchases().then((hasParkPalPlus: boolean) => {
        if(hasParkPalPlus) {
          hideBannerAdvertisement();
        }else{
          showBannerAdvertisement(hasParkPalPlus);
        }
        this.$store.dispatch('setParkPalPlus', hasParkPalPlus);
      });

      CapacitorPurchases.addListener('purchasesUpdate', (data: { purchases: Package; purchaserInfo: PurchaserInfo; }) => {
        console.log('Purchase update', data);
      })

      StatusBar.setStyle({
        style: Style.Light
      })
    }




  },
  mounted() {
    // Setup our banner advertisements.
    showBannerAdvertisement(this.settings.parkPalPlus);
  },
});
</script>