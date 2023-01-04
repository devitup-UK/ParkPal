<template>
  <IonPage ref="page">
    <IonHeader :style="'background: ' + settings.theme.header.background + ' !important;'">
      <IonToolbar color="transparent" :style="'background: ' + settings.theme.header.background + ' !important;'">
        <IonTitle :style="'background: ' + settings.theme.header.background + ' !important; border-width: 0 0px 2px; border-style: solid;border-color: ' + settings.theme.header.border + ' !important; color: '+ settings.theme.header.text + ' !important;'">Settings</IonTitle>
      </IonToolbar>
    </IonHeader>
    <IonContent :style="`background:${settings.theme.background} !important;`">
      <IonList lines="full" class="ion-margin-top settings-list" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
        <IonItem v-if="!settings.parkPalPlus" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <FontAwesomeIcon icon="moon" color="#EF86F4" class="settings-icon" fixed-width></FontAwesomeIcon>
          <IonLabel>Dark Mode</IonLabel>
          <IonToggle slot="end" v-model="darkMode" color="success"></IonToggle>
        </IonItem>
        <IonItem button v-if="settings.parkPalPlus" @click="navigate('settingsCustomTheming')" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <FontAwesomeIcon icon="paint-roller" color="#8b96cd" class="settings-icon" fixed-width></FontAwesomeIcon>
          <IonLabel>Custom Theming</IonLabel>
        </IonItem>
        <IonItem button @click="navigate('settingsManageDestinations')" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important; color: ${settings.theme.settings.settingText} !important;`">
          <FontAwesomeIcon icon="filter" color="#FFB857" class="settings-icon" fixed-width></FontAwesomeIcon>
          <IonLabel>Manage Destinations</IonLabel>
        </IonItem>
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <FontAwesomeIcon icon="plus" color="#F76C6C" class="settings-icon" fixed-width></FontAwesomeIcon>
          <IonLabel id="open-modal" @click="hideBannerAdvertisement">Subscribe to ParkPal+</IonLabel>
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
        <IonItem @click="rateApplication" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <FontAwesomeIcon icon="star" color="#ede6a0" class="settings-icon" fixed-width></FontAwesomeIcon>
          <IonLabel>Rate the App</IonLabel>
        </IonItem>
      </IonList>


      <IonModal ref="modal" trigger="open-modal" :can-dismiss="true" :presenting-element="presentingElement">
        <IonHeader :style="'background: ' + settings.theme.header.background + ' !important;'">
          <IonToolbar color="transparent" :style="'background: ' + settings.theme.header.background + ' !important;'">
            <IonTitle :style="'background: ' + settings.theme.header.background + ' !important; color: '+ settings.theme.header.text + ' !important;'">
              <span v-if="!settings.parkPalPlus">Subscribe to </span>
              <span v-else>Subscribed to </span>
              <span style="color:#C44E4E;">P</span><span style="color:#757FC9;">a</span><span style="color:#71B16F;">r</span>
              <span style="color:#E67AF4;">k</span><span style="color:#0A7266;">P</span><span style="color:#F59D24;">a</span><span style="color:#586DE2;">l</span><span style="color:#FF0000;">+</span></IonTitle>
            <IonButtons slot="end">
              <FontAwesomeIcon icon="times-circle" size="lg" :color="settings.theme.header.icons" @click="closeModal" fixed-width></FontAwesomeIcon>
            </IonButtons>
          </IonToolbar>
        </IonHeader>
        <ParkPalPlus @closeTriggered="$refs.modal.dismiss"></ParkPalPlus>
      </IonModal>

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

<script>
import { defineComponent } from "vue";
import {mapState} from "vuex";
import {
  IonPage,
  IonContent,
  IonHeader,
  IonToolbar,
  IonTitle,
  IonButtons,
  IonList,
  IonLabel,
  IonItem,
  IonModal,
  IonToggle,
  alertController
} from "@ionic/vue";
import {FontAwesomeIcon} from "@fortawesome/vue-fontawesome";
import ParkPalPlus from "@/components/Settings/ParkPalPlus.vue";
import {hideBannerAdvertisement, resumeBannerAdvertisement} from "@/handlers/advertisements.handler";
import { RateApp } from 'capacitor-rate-app';


export default defineComponent({
  name: "SettingsIndexView",
  components: {
    IonLabel,
    IonButtons,
    IonPage,
    IonContent,
    IonHeader,
    IonToolbar,
    IonTitle,
    IonItem,
    IonList,
    IonToggle,
    IonModal,
    ParkPalPlus,
    FontAwesomeIcon
  },
  computed: {
    ...mapState(['settings'])
  },
  data() {
    return {
      presentingElement: undefined,
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
    // Get all settings?
    if(!this.settings.parkPalPlus) {
      this.darkMode = this.settings.theme.darkMode;
    }
  },
  mounted() {
    this.presentingElement = this.$refs.page.$el;
  },
  methods: {
    hideBannerAdvertisement() {
      this.$store.dispatch('setModalOpen', true);
      hideBannerAdvertisement();
    },
    // Methods to go here.
    navigate(route) {
      this.$router.push({
        name: route,
        params: {
          transition: 'slide-right'
        }
      })
    },
    closeModal() {
      this.$store.dispatch('setModalOpen', false);
      resumeBannerAdvertisement(this.settings.parkPalPlus);
      this.$refs.modal.$el.dismiss(null, 'cancel');
    },
    rateApplication() {
      RateApp.requestReview();
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
              window.location = 'https://parkpal.co.uk/#getintouch'
            },
          },
        ],
      });

      await alert.present();
    }
  }
})
</script>