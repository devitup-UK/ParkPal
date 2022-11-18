<template>
  <IonPage>
    <IonHeader :style="'background: ' + settings.theme.header.background + ' !important;'">
      <IonToolbar color="transparent" :style="'background: ' + settings.theme.header.background + ' !important;'">
        <IonButtons slot="start">
          <IonButton @click="backToPreviousPage()">
            <FontAwesomeIcon icon="arrow-left" :color="settings.theme.header.icons" fixed-width></FontAwesomeIcon>
          </IonButton>
        </IonButtons>
        <IonTitle :style="'background: ' + settings.theme.header.background + ' !important; border-width: 0 0px 2px; border-style: solid;border-color: ' + settings.theme.header.border + ' !important; color: '+ settings.theme.header.text + ' !important;'">Edit Notification</IonTitle>
      </IonToolbar>
    </IonHeader>
    <IonContent :style="`background:${settings.theme.background} !important;`">
      <IonGrid>
        <Alert mode="warning" v-if="!notificationsEnabled" @click="requestNotificationPermissions">
          You must have Notifications enabled to receive Wait Time Notifications. Click here to enable them.
        </Alert>
        <Alert v-if="!settings.parkPalPlus && !adWatched" @click="watchAd">
          <span>Subscribe to ParkPal+ to set wait time notifications below 30 minutes or click here to watch an advertisement to set this notification below 30 minutes.</span>
        </Alert>
        <Alert v-if="!settings.parkPalPlus && adWatched" mode="success">
          <span>You've watched an ad and now have access to setting a wait time notification below 30 minutes.<br>Subscribe to ParkPal+ to avoid ads altogether!</span>
        </Alert>
        <IonRow>
          <IonCol class="attraction-wrapper">
            <AttractionComponent :attraction="attraction" :notification="notification" v-if="attraction != null"></AttractionComponent>
          </IonCol>
        </IonRow>

        <form class="filter-form">
          <IonRow class="select-filter" :style="`background:${settings.theme.selectionBoxBackground} !important; border-color: ${settings.theme.selectionBoxBorder} !important;`">
            <IonCol cols="6" class="select-filter__label">
              <h3 :style="`color: ${settings.theme.selectionBoxText} !important; background: transparent;`">Type</h3>
            </IonCol>
            <IonCol cols="6" class="select-filter__input">
              <IonSelect interface="action-sheet" cancelText="Cancel" v-model="criteria" @click="hideBannerAdvertisement" @ionDismiss="resumeBannerAdvertisement">
                <IonSelectOption v-for="criteria in definitions.criteria" :key="criteria.label" :value="criteria.value">{{ criteria.label }}</IonSelectOption>
              </IonSelect>
            </IonCol>
          </IonRow>
          <IonRow class="select-filter select-filter--wait" :style="`background:${settings.theme.selectionBoxBackground} !important; border-color: ${settings.theme.selectionBoxBorder} !important;`">
            <IonCol cols="6" class="select-filter__label">
              <h3 :style="`color: ${settings.theme.selectionBoxText} !important; background: transparent;`">Wait Time</h3>
            </IonCol>
            <IonCol cols="6" class="select-filter__input" @click="openWaitTimePicker()">
              <span>{{ waitTime }}</span>
            </IonCol>
          </IonRow>
          <IonRow class="filter-button">
            <IonCol>
              <IonButton expand="full" @click="editNotification" color="transparent" :style="`color: ${settings.theme.actionButtonText} !important; background: ${settings.theme.actionButtonBackground} !important;`">
                EDIT NOTIFICATION
              </IonButton>
            </IonCol>
          </IonRow>
          <IonRow class="filter-button">
            <IonCol>
              <IonButton expand="full" @click="backToPreviousPage" color="transparent" :style="`color: ${settings.theme.resetButtonText} !important; background: ${settings.theme.resetButtonBackground} !important;`">
                CANCEL
              </IonButton>
            </IonCol>
          </IonRow>
        </form>
      </IonGrid>
    </IonContent>
  </IonPage>
</template>

<script>
import { defineComponent } from "vue";
import {
  IonContent,
  IonPage,
  IonHeader,
  IonToolbar,
  IonButtons,
  IonButton,
  IonGrid,
  IonRow,
  IonCol,
  IonSelect,
  IonSelectOption,
  IonTitle, pickerController
} from "@ionic/vue";
import AttractionComponent from '@/components/Attraction.vue';

import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import {mapState} from "vuex";
import {WaitTimeFilterType} from "@/models/enums/WaitTimeFilterType";
import {WaitTimeFilterSort} from "@/models/enums/WaitTimeFilterSort";
import WaitTimeFilter from "@/models/store/WaitTimeFilter";
import {NotificationCriteria} from "@/models/enums/NotificationCriteria";
import AttractionTimer from "@/models/api/AttractionTimer";
import CreateNotificationRequest from "@/models/api/requests/notification/CreateNotificationRequest";
import EditNotificationRequest from "@/models/api/requests/notification/EditNotificationRequest";
import {hideBannerAdvertisement, resumeBannerAdvertisement, showRewardAdvertisement} from "@/events/advertisements.bus";
import {openAppNotificationSettings} from "@/handlers/notifications.handler";
import Alert from '@/components/Alert.vue';

export default defineComponent({
  name: "NotificationsEditView",
  components: {
    Alert,
    AttractionComponent,
    FontAwesomeIcon,
    IonContent,
    IonPage,
    IonHeader,
    IonToolbar,
    IonButtons,
    IonButton,
    IonTitle,
    IonGrid,
    IonRow,
    IonCol,
    IonSelect,
    IonSelectOption
  },
  computed: mapState({
    notificationsEnabled: state => state.notificationsEnabled,
    settings: state => state.settings,
    notifications: state => state.notifications,
    attraction: state => state.notificationHoldingArea.attraction,
    park: state => state.notificationHoldingArea.park
  }),
  data() {
    return {
      definitions: {
        criteria: [
          {
            value: 1,
            label: "Less Than"
          },
          {
            value: 2,
            label: "More Than"
          },
          {
            value: 3,
            label: "Equal To"
          },
        ]
      },
      notification: new AttractionTimer(),
      criteria: 1,
      waitTime: 35,
      adWatched: false,
      waitTimeOptions: []
    }
  },
  methods: {

    backToPreviousPage() {
      this.$router.push({
        path: this.$router.options.history.state['back'].toString(),
        replaceUrl: true
      })
    },

    async watchAd() {
      // console.log('Show advertisement.');
      showRewardAdvertisement().then((rewardItem) => {
        this.adWatched = true;
        this.setupWaitTimes();
      });
    },

    requestNotificationPermissions() {
      openAppNotificationSettings();
    },

    hideBannerAdvertisement() {
      hideBannerAdvertisement();
    },

    resumeBannerAdvertisement() {
      resumeBannerAdvertisement(this.settings.parkPalPlus);
    },

    setupWaitTimes() {
      this.waitTimeOptions = [{
        text: '5',
        value: 5
      }];

      // First we build up the wait time selection to be less than 180.
      while (this.waitTimeOptions[this.waitTimeOptions.length - 1].value < 180) {
        this.waitTimeOptions.push({
          text: (this.waitTimeOptions[this.waitTimeOptions.length - 1].value + 5).toString(),
          value: this.waitTimeOptions[this.waitTimeOptions.length - 1].value + 5
        })
      }

      // Then check if the criteria is set to LessThan.
      if(this.criteria == 1 && this.attraction.waitTime) {
        this.waitTimeOptions = this.waitTimeOptions.filter(a => a.value < this.attraction.waitTime);
      }

      if(!this.settings.parkPalPlus) {
        if(!this.adWatched) {
          this.waitTimeOptions = this.waitTimeOptions.filter(a => a.value >= 35);
        }
      }
    },

    async openWaitTimePicker() {
      this.setupWaitTimes();

      hideBannerAdvertisement();

      const picker = await pickerController.create({
        columns: [
          {
            name: 'waitTime',
            options: this.waitTimeOptions
          },
        ],
        buttons: [
          {
            text: 'Cancel',
            role: 'cancel',
            handler: () => {
              resumeBannerAdvertisement(this.settings.parkPalPlus);
            }
          },
          {
            text: 'Confirm',
            handler: (value) => {
              this.waitTime = value.waitTime.value;
              resumeBannerAdvertisement(this.settings.parkPalPlus);
            },
          },
        ],
      });
      await picker.present();
    },

    editNotification() {
      let notificationToEdit = new EditNotificationRequest({
        attractionTimerId: this.notification.timer.attractionTimerId,
        criteriaType: this.criteria,
        waitTime: this.waitTime
      });

      this.$store.dispatch('editNotification', notificationToEdit);

      this.backToPreviousPage();
    }
  },
  beforeMount() {
    // Set the notification for the view.
    this.notification = this.notifications.filter(a => a.timer.attractionTimerId == this.$route.params.attractionTimerId)[0];
    this.criteria = this.notification.timer.criteriaType;
    this.waitTime = this.notification.timer.waitTime;
  },
  beforeUnmount() {
    this.$store.commit('clearNotificationHoldingArea');
  },

})
</script>

<style lang="scss" scoped>
.select-filter {
  border-width: 0 0 1px;
  border-style: solid;
  border-color: #D5D5D5;

  &:nth-child(1) {
    border-width: 1px 0;
  }

  &:nth-child(2) {
    margin-bottom: 16px;
  }
}

.select-filter, .select-filter__label > h3 {
  background: #FFF;
  font-size: 14px;
  color: #9D9D9D;

  .select-filter__label {
    display: flex;
    align-items: center;
    padding-left: 16px;

    h3 {
      margin: 0;
      font-weight: 400;
    }
  }
}

ion-grid {
  padding: 0;
}

ion-select {
  text-align: right;

  &::part(icon) {
    display: none !important;
  }
}


.select-filter--wait {
  padding: 10px 10px 10px 0;

  .select-filter__input {
    text-align: right;
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

.attraction-wrapper {
  margin-top: 10px;
}
</style>