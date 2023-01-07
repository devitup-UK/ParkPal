<template>
<!--  <nav>-->
  <IonApp>
    <div class="notification-permissions-request" v-if="!settings.requestedNotifications">
      <div class="notification-permissions-request__close" @click="denyPermissions">
        <FontAwesomeIcon icon="times-circle" color="#000" size="2x"></FontAwesomeIcon>
      </div>
      <div class="notification-permissions-request__content">
        <img src="@/assets/request-notification-permissions.svg">
        <p :style="'color: ' + settings.theme.text + ' !important;'">Before using ParkPal, you must enable Push Notifications to receive wait time notifications, click the button below to accept Push Notifications. You can disable these later if you wish.</p>
        <div class="filter-button">
          <IonRow class="filter-button">
            <IonCol>
              <IonButton expand="full" @click="requestPermissions" color="transparent" :style="`color: ${settings.theme.actionButtonText} !important; background: ${settings.theme.actionButtonBackground} !important;`">
                ENABLE PUSH NOTIFICATIONS
              </IonButton>
            </IonCol>
          </IonRow>
          <IonRow class="filter-button">
            <IonCol>
              <IonButton expand="full" @click="denyPermissions" color="transparent" :style="`color: ${settings.theme.resetButtonText} !important; background: ${settings.theme.resetButtonBackground} !important;`">
                NO THANKS, MAYBE LATER
              </IonButton>
            </IonCol>
          </IonRow>
        </div>
      </div>
    </div>
    <div class="content" :class="{ 'content--noads': this.settings.parkPalPlus }" :style="`margin-bottom: ${adHeight}px;`">
      <RouterView v-slot="{ Component, route }">
        <transition :name="route.params.transition">
          <component :is="Component" />
        </transition>
      </RouterView>
      <div class="advertisement-placeholder" v-if="!this.settings.parkPalPlus && this.isApp" :style="`bottom: -${adHeight}px; height: ${adHeight}px; background:${settings.theme.background} !important; color: ${settings.theme.text} !important;`">
        <FontAwesomeIcon icon="spinner" spin fixed-width></FontAwesomeIcon>
        <p>Loading advertisements...</p>
      </div>
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
  </IonApp>
</template>

<style lang="scss">
@import "~placeholder-loading/src/scss/placeholder-loading";

.ph-row {
  flex-direction: inherit;
}

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

.ion-page {
  height: 100%;
  width: 100%;
  top: unset !important;
  right: unset !important;
  bottom: unset !important;
  left: unset !important;
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

  &.content--noads {
    margin-bottom: 0 !important;
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


.slide-right-enter-active,
.slide-right-leave-active,
.slide-left-enter-active,
.slide-left-leave-active{
  transition: all 0.15s ease-out;
}

.slide-right-enter-to {
  position: absolute;
  right: 0 !important;
}
.slide-right-enter-from {
  position: absolute;
  right: -100% !important;
}
.slide-right-leave-to {
  position: absolute;
  left: -100% !important;
}
.slide-right-leave-from {
  position: absolute;
  left: 0 !important;
}

.slide-left-enter-to {
  position: absolute;
  left: 0 !important;
}
.slide-left-enter-from {
  position: absolute;
  left: -100% !important;
}
.slide-left-leave-to {
  position: absolute;
  right: -100% !important;
}
.slide-left-leave-from {
  position: absolute;
  right: 0 !important;
}

.advertisement-placeholder {
  position: absolute;
  width: 100%;
  display: flex;
  font-size: 12px;
  flex-direction: column;
  justify-content: center;
  align-items: center;

  p {
    margin-bottom: 0;
    margin-top: 4px;
  }
}

.filter-button {

  ion-col {
    padding: 0;

    ion-button {
      margin: 0;
      font-weight: 300;
      font-size: 14px;
    }
  }
}

.notification-permissions-request {
  position: absolute;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
  z-index: 1000;
  background: #FCFCFC;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-direction: column;

  .notification-permissions-request__close {
    position: absolute;
    top: calc(var(--ion-safe-area-top, 0) + 10px);
    right: 10px;
  }

  p {
    margin: 10px 20px 20px;
    font-size: 14px;
  }
}
</style>

<script lang="ts">
import {IonApp, IonButton, IonLabel, IonTabBar, IonTabButton, IonRow, IonCol} from '@ionic/vue';
import {PushNotifications} from '@capacitor/push-notifications';
import {ScreenOrientation} from "@awesome-cordova-plugins/screen-orientation";
import {defineComponent} from 'vue';
import {FontAwesomeIcon} from '@fortawesome/vue-fontawesome';
import {mapState} from 'vuex';
import {tokenService} from "@/services/token.service";
import {RouterView} from "vue-router";
import {hideBannerAdvertisement, initialiseAdvertisements, showBannerAdvertisement} from '@/handlers/advertisements.handler';
import {
  requestNotificationPermissions,
  saveSubscriptionToDatabase,
  setupOneSignal
} from "@/handlers/notifications.handler";
import {App} from '@capacitor/app';
import {StatusBar, Style} from "@capacitor/status-bar";
import parkpalplusHandler from "@/handlers/parkpalPlus.handler";
import {CapacitorPurchases, Package, PurchaserInfo} from "@capgo/capacitor-purchases";
import {PurchasesPackage} from "cordova-plugin-purchases";
import parkpalPlusHandler from "@/handlers/parkpalPlus.handler";
import {subscriptionService} from "@/services/subscription.service";
import VoucherRequest from "@/models/api/requests/subscription/VoucherRequest";


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
    IonButton,
    IonRow,
    IonCol,
    RouterView
  },
  computed: {
    ...mapState(['settings', 'isApp', 'notificationsEnabled', 'modalOpen', 'adHeight'])
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
    },

    requestPermissions() {
      // Setup OneSignal with all of our details, this could be a first ever launch of our app or a user opening the app after closing it.
      setupOneSignal().then(() => {
        // OneSignal setup complete and verified.
        requestNotificationPermissions().then(() => {
          // We have received word that they have accepted permissions, we will now save the OneSignal subscription to the database.
          saveSubscriptionToDatabase().then(() => {
            this.$store.dispatch('setNotificationsEnabled', true);
            this.$store.dispatch('setNotificationsRequested', true);
            initialiseAdvertisements();
          }).catch(() => {
            this.$store.dispatch('setNotificationsEnabled', false);
            initialiseAdvertisements();
          });
        }).catch(() => {
          this.$store.dispatch('setNotificationsEnabled', false);
          initialiseAdvertisements();
        })
      });
    },

    denyPermissions() {
      this.$store.dispatch('setNotificationsRequested', true);
      initialiseAdvertisements();
    }
  },

  // We need to check if our local storage has our settings before, this is where all of our data is stored even after the app has been force closed.
  beforeMount() {

    this.$store.dispatch('configureStorage');

    // First we need to check if we have an API token set in our settings, this would have come from our localStorage if this is a returning user.
    if(!this.settings.apiToken) {
      this.generateAndSaveToken();
    }

    // Setup our OneSignalProperties
    if(this.isApp) {
      ScreenOrientation.lock(ScreenOrientation.ORIENTATIONS.PORTRAIT);
      parkpalplusHandler.setDebugLogLevel();
      parkpalplusHandler.initialisePurchases();


      // If the app has been resumed and PushNotifications are no longer granted, we can set the notifications flag to false.
      App.addListener('resume',() => {
        // Check the permissions.
        PushNotifications.checkPermissions().then((permissions) => {
          if(permissions.receive == 'granted') {
            setupOneSignal().then(() => {
              saveSubscriptionToDatabase().then(() => {
                this.$store.dispatch('setNotificationsEnabled', true);
              }).catch(() => {
                this.$store.dispatch('setNotificationsEnabled', false);
              });
            });
          }else{
            this.$store.dispatch('setNotificationsEnabled', false);
          }
        })

        // See if the user still has a subscription active.
        parkpalPlusHandler.getPurchases().then((activeSubscriptions) => {
          this.$store.dispatch('setParkPalPlus', activeSubscriptions.length);
          if(activeSubscriptions.length) {
            hideBannerAdvertisement();
          }else{
            if(!this.modalOpen) {
              showBannerAdvertisement(activeSubscriptions.length > 0);
            }
          }
        })
      })

      // Initialise our advertisements right at the start of the application.
      if(this.settings.requestedNotifications) {
        initialiseAdvertisements();
      }

      parkpalplusHandler.getProducts().then((products: Array<PurchasesPackage>) => {
        this.$store.dispatch('setProducts', products);
      });

      parkpalPlusHandler.getPurchases().then((activeSubscriptions) => {
        this.$store.dispatch('setParkPalPlus', activeSubscriptions.length);
      })

      // If we have a voucher in our settings, we need to verify it.
      // if(this.settings.voucher != undefined) {
      //   subscriptionService.verifyVoucher({
      //     code: this.settings.voucher
      //   }).then(() => {
      //     this.$store.dispatch('setParkPalPlus', true);
      //     hideBannerAdvertisement();
      //   }).catch(() => {
      //     this.$store.dispatch('setParkPalPlus', false);
      //   })
      // }

      StatusBar.setStyle({
        style: Style.Light
      })
    }




  },
  mounted() {
    // Setup our banner advertisements.
    setTimeout(() => {
      showBannerAdvertisement(this.settings.parkPalPlus);
    }, 400);
  },
});
</script>