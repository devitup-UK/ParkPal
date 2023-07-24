<template>
  <IonPage ref="page">
    <IonHeader :style="'background: ' + settings.theme.header.background + ' !important;'">
      <IonToolbar color="transparent" :style="'background: ' + settings.theme.header.background + ' !important;'">
        <IonTitle :style="'background: ' + settings.theme.header.background + ' !important; border-width: 0 0px 2px; border-style: solid;border-color: ' + settings.theme.header.border + ' !important; color: '+ settings.theme.header.text + ' !important;'">Settings</IonTitle>
      </IonToolbar>
    </IonHeader>
    <IonContent :style="`background:${settings.theme.background} !important;`">
      <IonList lines="full" class="ion-margin-top settings-list" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <FontAwesomeIcon icon="moon" color="#EF86F4" class="settings-icon" fixed-width></FontAwesomeIcon>
          <IonLabel>Dark Mode</IonLabel>
          <IonToggle slot="end" v-model="darkMode" color="success"></IonToggle>
        </IonItem>
        <IonItem button @click="navigate('settingsCustomTheming')" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <FontAwesomeIcon icon="paint-roller" color="#8b96cd" class="settings-icon" fixed-width></FontAwesomeIcon>
          <IonLabel>Custom Theming</IonLabel>
        </IonItem>
        <IonItem button @click="navigate('settingsManageDestinations')" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important; color: ${settings.theme.settings.settingText} !important;`">
          <FontAwesomeIcon icon="filter" color="#FFB857" class="settings-icon" fixed-width></FontAwesomeIcon>
          <IonLabel>Manage Destinations</IonLabel>
        </IonItem>
        <IonItem v-if="!settings.noAds" @click="purchaseNoAds" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <FontAwesomeIcon icon="plus" color="#F76C6C" class="settings-icon" fixed-width></FontAwesomeIcon>
          <IonLabel>Remove Ads</IonLabel>
        </IonItem>
        <IonItem @click="restorePurchases" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <FontAwesomeIcon icon="cart-shopping" color="#F76C6C" class="settings-icon" fixed-width></FontAwesomeIcon>
          <IonLabel>Restore Purchases</IonLabel>
        </IonItem>
      </IonList>

      <IonList lines="full" class="ion-margin-top settings-list" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
        <IonItem button @click="navigate('settingsAboutAndFAQs')" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <FontAwesomeIcon icon="at" color="#97A9AF" class="settings-icon" fixed-width></FontAwesomeIcon>
          <IonLabel>About & FAQs</IonLabel>
        </IonItem>
        <IonItem href="https://parkpal.co.uk" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <FontAwesomeIcon icon="link" color="#5eaf6d" class="settings-icon" fixed-width></FontAwesomeIcon>
          <IonLabel>ParkPal Website</IonLabel>
        </IonItem>
        <IonItem @click="feedbackMessage" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <FontAwesomeIcon icon="comments" color="#3f3f54" class="settings-icon" fixed-width></FontAwesomeIcon>
          <IonLabel>Leave Feedback</IonLabel>
        </IonItem>
      </IonList>

      <IonList lines="full" class="ion-margin-top settings-list" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
        <IonItem href="https://www.apple.com/legal/internet-services/itunes/dev/stdeula/" target="_blank" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <FontAwesomeIcon icon="book" color="#f7adf1" class="settings-icon" fixed-width></FontAwesomeIcon>
          <IonLabel>Terms of Use</IonLabel>
        </IonItem>
        <IonItem href="https://parkpal.co.uk/#privacy-policy" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <FontAwesomeIcon icon="user-secret" color="#668593" class="settings-icon" fixed-width></FontAwesomeIcon>
          <IonLabel>Privacy Policy</IonLabel>
        </IonItem>
      </IonList>

      <IonList lines="full" class="ion-margin-top settings-list" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
        <IonItem href="https://www.facebook.com/profile.php?id=100092728901540" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <FontAwesomeIcon :icon="['fa-brands', 'facebook']" color="#4267B2" class="settings-icon" fixed-width></FontAwesomeIcon>
          <IonLabel>Facebook</IonLabel>
        </IonItem>
        <IonItem href="https://twitter.com/ParkPalUK" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <FontAwesomeIcon :icon="['fa-brands','twitter']" color="#1DA1F2" class="settings-icon" fixed-width></FontAwesomeIcon>
          <IonLabel>Twitter</IonLabel>
        </IonItem>
        <IonItem href="https://www.instagram.com/parkpalapp/" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <FontAwesomeIcon :icon="['fa-brands','instagram']" color="#E1306C" class="settings-icon" fixed-width></FontAwesomeIcon>
          <IonLabel>Instagram</IonLabel>
        </IonItem>
      </IonList>

    </IonContent>
  </IonPage>
</template>

<style lang="scss" scoped>
ion-label {
  margin-bottom: 6px !important;
}

ion-item::part(native) {
  background: inherit;
  font-size: 14px;
  color: inherit;
  padding-left: 12px;
  border-color: inherit;
}

ion-item::part(detail-icon) {
  color: inherit;
}

.item-native {
  border-color: inherit !important;
}

.settings-list {
  border-top: 1px solid var(--ion-item-border-color, var(--ion-border-color, var(--ion-color-step-250, #c8c7cc)));;
}

.settings-icon {
  font-size: 20px;
  margin-right: 5px;
}
</style>

<script lang="ts">
import { defineComponent } from "vue";
import {mapState} from "vuex";
import {
  IonPage,
  IonContent,
  IonHeader,
  IonToolbar,
  IonTitle,
  IonList,
  IonLabel,
  IonItem,
  IonToggle,
  alertController
} from "@ionic/vue";
import {FontAwesomeIcon} from "@fortawesome/vue-fontawesome";
import productsHandler from '@/handlers/products.handler';


export default defineComponent({
  name: "SettingsIndexView",
  components: {
    IonLabel,
    IonPage,
    IonContent,
    IonHeader,
    IonToolbar,
    IonTitle,
    IonItem,
    IonList,
    IonToggle,
    FontAwesomeIcon
  },
  computed: {
    ...mapState(['settings'])
  },
  data() {
    return {
      darkMode: false
    }
  },
  watch: {
    darkMode(value) {
      if(value) {
        this.$store.dispatch('setDarkMode');
      }else{
        this.$store.dispatch('setLightMode');
      }
    }
  },
  beforeMount() {
      this.darkMode = this.settings.theme.darkMode;
  },
  methods: {
    // Methods to go here.
    navigate(route: string) {
      this.$router.push({
        name: route,
        params: {
          transition: 'slide-right'
        }
      })
    },
    restorePurchases() {
      productsHandler.restorePurchases();
    },
    purchaseNoAds() {
      productsHandler.purchaseProduct("noAds").then();
    },
    async feedbackMessage() {
      const alert = await alertController.create({
        header: 'Give Feedback',
        message: 'You will be redirected to the ParkPal website to leave feedback  using the feedback form, would you like to continue?',
        buttons: [
          {
            text: 'Cancel',
            role: 'cancel'
          },
          {
            text: 'OK',
            role: 'confirm',
            handler: () => {
              window.location.href =  'https://parkpal.co.uk/#getintouch';
            },
          },
        ],
      });

      await alert.present();
    },
  }
})
</script>