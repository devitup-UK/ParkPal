<template>
  <IonPage>
    <IonHeader :style="'background: ' + settings.theme.header.background + ' !important;'">
      <IonToolbar color="transparent" :style="'background: ' + settings.theme.header.background + ' !important;'">
        <IonButtons slot="start">
          <IonButton @click="backToPreviousPage">
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
            <NotificationComponent :notification="notification" :destinationName="getDestinationName(notification.properties.parkId)"></NotificationComponent>
          </IonCol>
        </IonRow>

        <form class="filter-form">
          <SelectBox label="Criteria" v-model="notification.properties.criteriaType" @click="hideBannerAdvertisement" @dismiss="resumeBannerAdvertisement" :options="definitions.criteria"></SelectBox>
          <PickerComponent label="Wait Time" :value="notification.properties.waitTime" v-model="notification.properties.waitTime" :columns="[
          {
            name: 'waitTime',
            options: this.waitTimeOptions,
            selectedIndex: this.waitTimeOptions.findIndex(a => a.value == this.notification.properties?.waitTime)

          },
        ]" :buttons="[
          {
            text: 'Cancel',
            role: 'cancel',
            handler: () => {
              resumeBannerAdvertisement(this.settings.parkPalPlus);
            }
          },
        ]"></PickerComponent>
          <IonRow class="filter-button">
            <IonCol>
              <IonButton expand="full" @click="editNotification" color="transparent" :style="`color: ${settings.theme.actionButtonText} !important; background: ${settings.theme.actionButtonBackground} !important;`">
                SAVE NOTIFICATION
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

<script lang="ts">
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
import PickerComponent from "@/components/custom-inputs/Picker.vue";

import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import {mapState} from "vuex";
import EditNotificationRequest from "@/models/api/requests/notification/EditNotificationRequest";
import {hideBannerAdvertisement, resumeBannerAdvertisement, showRewardAdvertisement} from "@/handlers/advertisements.handler";
import {openAppNotificationSettings} from "@/handlers/notifications.handler";
import Alert from '@/components/Alert.vue';
import Notification from "@/models/api/Notification";
import {RootState} from "@/store/types";
import Attraction from "@/models/api/Attraction";
import Settings from "@/models/store/Settings";
import Park from "@/models/api/Park";
import NotificationComponent from "@/components/Notification.vue";
import SelectBox from "@/components/custom-inputs/SelectBox.vue";
import {themeparkService} from "@/services/themepark.service";
import {AxiosResponse} from "axios";
import Destination from "@/models/api/Destination";

export default defineComponent({
  name: "NotificationsEditView",
  components: {
    SelectBox,
    NotificationComponent,
    Alert,
    PickerComponent,
    // AttractionComponent,
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
    IonCol
  },
  computed: {
    ...mapState(['notificationsEnabled', 'settings', 'notifications']),
    waitTimeOptions() {
      let waitTimeOptions = [{
        text: '5',
        value: 5
      }];

      // First we build up the wait time selection to be less than 180.
      while (waitTimeOptions[waitTimeOptions.length - 1].value < 180) {
        waitTimeOptions.push({
          text: (waitTimeOptions[waitTimeOptions.length - 1].value + 5).toString(),
          value: waitTimeOptions[waitTimeOptions.length - 1].value + 5
        })
      }

      // Then check if the criteria is set to LessThan.
      // if(this.criteria === 1 && this.attraction.waitTime) {
      //   this.waitTimeOptions = this.waitTimeOptions.filter(a => a.value < this.attraction.waitTime);
      // }

      if(!this.settings.parkPalPlus) {
        if(!this.adWatched) {
          waitTimeOptions = waitTimeOptions.filter(a => a.value >= 35);
        }
      }

      return waitTimeOptions;
    }
  },
  data(): { definitions: { criteria: Array<{ value: string | number, label: string }> }, notification: Notification, adWatched: boolean, destinations: Array<Destination> } {
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
      notification: new Notification(),
      adWatched: false,
      destinations: []
    }
  },
  methods: {

    backToPreviousPage() {
      if(this.$router.options.history.state['back']) {
        const backRoute = this.$router.getRoutes().find(a => a.path == this.$router.options.history.state['back']);
        if(backRoute) {
          this.$router.push({
            name: backRoute.name,
            params: {
              transition: 'slide-left'
            }
          })
        }
      }
    },

    async watchAd() {
      showRewardAdvertisement().then(() => {
        this.adWatched = true;
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

    editNotification() {
      let notificationToEdit = new EditNotificationRequest({
        notificationId: this.notification.properties?.itemId,
        criteriaType: this.notification.properties.criteriaType,
        waitTime: this.notification.properties.waitTime
      });

      this.$store.dispatch('editNotification', notificationToEdit);

      this.backToPreviousPage();
    },
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
  },
  beforeMount() {
    this.getDestinations();
    // Set the notification for the view.
    if(this.notifications.length) {
      let notificationId = ((this.$route.params.notificationId as unknown) as number);
      if(notificationId) {
        this.notification = new Notification(this.notifications.find((a: Notification) => a.properties?.itemId == notificationId));
      }
    }
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