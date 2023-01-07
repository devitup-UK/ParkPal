<template>
  <IonPage>
    <IonHeader :style="'background: ' + settings.theme.header.background + ' !important;'">
      <IonToolbar color="transparent" :style="'background: ' + settings.theme.header.background + ' !important;'">
        <IonButtons slot="start">
          <IonButton @click="backToSettings">
            <FontAwesomeIcon icon="arrow-left" :color="settings.theme.header.icons" fixed-width></FontAwesomeIcon>
          </IonButton>
        </IonButtons>
        <IonTitle :style="'background: ' + settings.theme.header.background + ' !important; border-width: 0 0px 2px; border-style: solid;border-color: ' + settings.theme.header.border + ' !important; color: '+ settings.theme.header.text + ' !important;'">About & FAQs</IonTitle>
      </IonToolbar>
    </IonHeader>
    <IonContent :style="`background:${settings.theme.background} !important;`">
      <IonList lines="full" class="ion-margin-top settings-list" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel slot="start">Developer</IonLabel>
          <IonLabel slot="end">DevItUp</IonLabel>
        </IonItem>
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel slot="start">Version</IonLabel>
          <IonLabel slot="end">1.1.1</IonLabel>
        </IonItem>
      </IonList>

      <IonAccordionGroup class="ion-margin-top settings-list">
        <IonAccordion>
          <IonItem slot="header" lines="full" class="faq-item" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
            <IonLabel>What is ParkPal?</IonLabel>
          </IonItem>
          <div class="ion-padding faq-answer" slot="content">
            ParkPal is a wait time tracking application that allows you to set notifications to alert you when a wait time is more than, less than or equal to your desired criteria.
          </div>
        </IonAccordion>
        <IonAccordion>
          <IonItem slot="header" lines="full" class="faq-item" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
            <IonLabel>Why should I use ParkPal?</IonLabel>
          </IonItem>
          <div class="ion-padding faq-answer" slot="content">
            ParkPal can assist you while you enjoy your time at your favourite theme park by helping you to get notified if your favourite attraction meets your wait time expectations. You can setup wait time notifications that will notify you when the wait time of your chosen attraction is <strong>more than</strong>, <strong>less than</strong> or <strong>equal to</strong> your desired wait time.
          </div>
        </IonAccordion>
        <IonAccordion>
          <IonItem slot="header" lines="full" class="faq-item" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
            <IonLabel>Does ParkPal need an Internet connection?</IonLabel>
          </IonItem>
          <div class="ion-padding faq-answer" slot="content">
            Yes, ParkPal relies on fetching data from all theme parks that you have wait time notifications enabled for and relies on server communications to display available destinations, parks and wait times. You do not need constant internet connection to receive notifications, these happen in the background.
          </div>
        </IonAccordion>
        <IonAccordion>
          <IonItem slot="header" lines="full" class="faq-item" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
            <IonLabel>My favourite destination is missing!</IonLabel>
          </IonItem>
          <div class="ion-padding faq-answer" slot="content">
            We are so sorry that we don't have your favourite destination, feel free to drop us a message to <strong>support@parkpal.co.uk</strong> to let us know and we will do our best to get it added to the list.
          </div>
        </IonAccordion>
        <IonAccordion>
          <IonItem slot="header" lines="full" class="faq-item" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
            <IonLabel>Why do you have advertisements?</IonLabel>
          </IonItem>
          <div class="ion-padding faq-answer" slot="content">
            To send you notifications about wait times, we need servers and servers cost to run, advertisements help us by sustaining the server costs and development costs to keep ParkPal running. Any advertisement revenue earned is also poured into improving the ParkPal application.
          </div>
        </IonAccordion>
        <IonAccordion>
          <IonItem slot="header" lines="full" class="faq-item" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
            <IonLabel>Why should I subscribe to ParkPal+?</IonLabel>
          </IonItem>
          <div class="ion-padding faq-answer" slot="content">
            ParkPal+ offers you the ability to use the application with no advertisements, set less than wait time notifications below 35 minutes, removes any limits on the amount of notifications you can have at one time, offers you the ability to theme your ParkPal experience in anyway you like. You can access all features without ParkPal+ except custom theming and unlimited notifications by watching advertisements.
          </div>
        </IonAccordion>
        <IonAccordion>
          <IonItem slot="header" lines="full" class="faq-item" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
            <IonLabel>Are there future plans for ParkPal?</IonLabel>
          </IonItem>
          <div class="ion-padding faq-answer" slot="content">
            We aim to release bi-weekly updates to the ParkPal application if improvements are reported and need to be fixed, we are always happy to hear improvements to the application and welcome criticism on anything we can fix or add to the application. In a nutshell, we fully expect ParkPal to keep growing for the foreseeable future.
          </div>
        </IonAccordion>
        <IonAccordion>
          <IonItem slot="header" lines="full" class="faq-item" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
            <IonLabel>I have an idea that can improve ParkPal!</IonLabel>
          </IonItem>
          <div class="ion-padding faq-answer" slot="content">
            Submit your idea to <strong>support@parkpal.co.uk</strong> and we will certainly have a look and if it sounds promising, we will work our hardest to get it implemented as soon as we can.
          </div>
        </IonAccordion>
      </IonAccordionGroup>
      <div class="copyright" :style="`color: ${settings.theme.text} !important;`">
        <p>&copy; 2022 DevItUp / ParkPal</p>
      </div>
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

.settings-list {
  border-top: 1px solid var(--ion-item-border-color, var(--ion-border-color, var(--ion-color-step-250, #c8c7cc)));

  &:nth-child(2) {
    border-bottom:1px solid var(--ion-item-border-color, var(--ion-border-color, var(--ion-color-step-250, #c8c7cc)));;

  }
}

.settings-icon {
  font-size: 20px;
  margin-right: 5px;
}

.faq-answer {
  background: #fff;
  text-align: left;
  font-size: 14px;
  color: #6b6b6b;
  //border-bottom:1px solid var(--ion-item-border-color, var(--ion-border-color, var(--ion-color-step-250, #c8c7cc)));;
}
.copyright {
  font-size: 14px;
}
</style>

<script lang="ts">
import { defineComponent } from "vue";
import {mapState} from "vuex";
import { IonPage, IonContent, IonHeader, IonToolbar, IonTitle, IonButton, IonButtons, IonList, IonItem, IonLabel, IonAccordionGroup, IonAccordion } from "@ionic/vue";
import {FontAwesomeIcon} from "@fortawesome/vue-fontawesome";

export default defineComponent({
  name: "SettingsAboutAndFAQsView",
  components: {
    IonButtons,
    IonButton,
    IonPage,
    IonContent,
    IonHeader,
    IonToolbar,
    IonTitle,
    IonList,
    IonItem,
    IonLabel,
    IonAccordion,
    IonAccordionGroup,
    FontAwesomeIcon
  },
  computed: {
    ...mapState(['settings'])
  },
  beforeMount() {
    // Get all settings?
  },
  methods: {
    // Methods to go here.
    backToSettings() {
      this.$router.push({
        name: 'settings',
        params: {
          transition: 'slide-left'
        }
      })
    }
  }
})
</script>