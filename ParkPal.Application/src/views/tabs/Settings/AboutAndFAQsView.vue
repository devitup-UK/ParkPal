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
          <IonLabel slot="end">1.0</IonLabel>
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
            Answer to go here.
          </div>
        </IonAccordion>
        <IonAccordion>
          <IonItem slot="header" lines="full" class="faq-item" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
            <IonLabel>Does ParkPal need an Internet connection?</IonLabel>
          </IonItem>
          <div class="ion-padding faq-answer" slot="content">
            Answer to go here.
          </div>
        </IonAccordion>
        <IonAccordion>
          <IonItem slot="header" lines="full" class="faq-item" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
            <IonLabel>My favourite destination is missing!</IonLabel>
          </IonItem>
          <div class="ion-padding faq-answer" slot="content">
            Answer to go here.
          </div>
        </IonAccordion>
        <IonAccordion>
          <IonItem slot="header" lines="full" class="faq-item" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
            <IonLabel>Why do you have advertisements?</IonLabel>
          </IonItem>
          <div class="ion-padding faq-answer" slot="content">
            Answer to go here.
          </div>
        </IonAccordion>
        <IonAccordion>
          <IonItem slot="header" lines="full" class="faq-item" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
            <IonLabel>Why should I subscribe to ParkPal+?</IonLabel>
          </IonItem>
          <div class="ion-padding faq-answer" slot="content">
            Answer to go here.
          </div>
        </IonAccordion>
        <IonAccordion>
          <IonItem slot="header" lines="full" class="faq-item" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
            <IonLabel>Are there future plans for ParkPal?</IonLabel>
          </IonItem>
          <div class="ion-padding faq-answer" slot="content">
            Answer to go here.
          </div>
        </IonAccordion>
        <IonAccordion>
          <IonItem slot="header" lines="full" class="faq-item" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
            <IonLabel>I have an idea for that can improve ParkPal!</IonLabel>
          </IonItem>
          <div class="ion-padding faq-answer" slot="content">
            Answer to go here.
          </div>
        </IonAccordion>
      </IonAccordionGroup>
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
</style>

<script lang="ts">
import { defineComponent } from "vue";
import AttractionComponent from "../../../components/Attraction.vue";
import {mapGetters, mapState} from "vuex";
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
    // AttractionComponent,
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
        name: 'settings'
      })
    }
  }
})
</script>