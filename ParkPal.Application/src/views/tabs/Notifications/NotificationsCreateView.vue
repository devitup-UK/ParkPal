<template>
  <IonPage>
    <IonHeader :style="'background: ' + settings.theme.header.background + ' !important;'">
      <IonToolbar color="transparent" :style="'background: ' + settings.theme.header.background + ' !important;'">
        <IonButtons slot="start">
          <IonButton @click="backToWaitTimes()">
            <FontAwesomeIcon icon="arrow-left" :color="settings.theme.header.icons" fixed-width></FontAwesomeIcon>
          </IonButton>
        </IonButtons>
        <IonTitle :style="'background: ' + settings.theme.header.background + ' !important; border-width: 0 0px 2px; border-style: solid;border-color: ' + settings.theme.header.border + ' !important; color: '+ settings.theme.header.text + ' !important;'">Create A Notification</IonTitle>
      </IonToolbar>
    </IonHeader>
    <IonContent :style="`background:${settings.theme.background} !important;`">
      <IonGrid>
        <Alert mode="warning" v-if="!notificationsEnabled" @click="requestNotificationPermissions">
          You must have Notifications enabled to receive Wait Time Notifications. Click here to enable them.
        </Alert>
        <Alert v-if="!settings.noAds && !adWatched" @click="watchAd">
          <span>Purchase Ad Removal to set wait time notifications below 30 minutes or click here to watch an advertisement to set this notification below 35 minutes.</span>
        </Alert>
        <Alert v-if="!settings.noAds && adWatched" mode="success">
          <span>You've watched an ad and now have access to setting a wait time notification below 35 minutes.<br>Purchase Ad Removal to remove ads altogether!</span>
        </Alert>
        <IonRow>
          <IonCol class="attraction-wrapper">
            <NotificationComponent :notification="notification" :destinationName="getDestinationName(notification.properties.parkId)"></NotificationComponent>
          </IonCol>
        </IonRow>

        <form class="filter-form">
          <SelectBox label="Criteria" v-model="notification.properties.criteriaType" @click="hideBannerAdvertisement" @dismiss="resumeBannerAdvertisement" :options="definitions.criteria"></SelectBox>
          <PickerComponent label="Wait Time" v-model="notification.properties.waitTime" :columns="[
          {
            name: 'waitTime',
            options: this.waitTimeOptions,
            selectedIndex: this.waitTimeOptions.findIndex(a => a.value === this.notification.properties?.waitTime)

          },
        ]" :buttons="[
          {
            text: 'Cancel',
            role: 'cancel',
            handler: () => {
              this.$store.dispatch('setModalOpen', false)
              resumeBannerAdvertisement();
            }
          },
        ]"></PickerComponent>
          <IonRow class="filter-button">
            <IonCol>
              <IonButton expand="full" @click="createNotification"  color="transparent" :style="`color: ${settings.theme.actionButtonText} !important; background: ${settings.theme.actionButtonBackground} !important;`">
                CREATE NOTIFICATION
              </IonButton>
            </IonCol>
          </IonRow>
          <IonRow class="filter-button">
            <IonCol>
              <IonButton expand="full" @click="backToWaitTimes"  color="transparent" :style="`color: ${settings.theme.resetButtonText} !important; background: ${settings.theme.resetButtonBackground} !important;`">
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
import Alert from '@/components/Alert.vue';

import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import {mapState} from "vuex";
import CreateNotificationRequest from "@/models/api/requests/notification/CreateNotificationRequest";
import {hideBannerAdvertisement, resumeBannerAdvertisement, showRewardAdvertisement} from "@/handlers/advertisements.handler";
import {
  openAppNotificationSettings,
  requestNotificationPermissions, saveSubscriptionToDatabase,
  setupOneSignal
} from "@/handlers/notifications.handler";
import NotificationComponent from "@/components/Notification.vue";
import SelectBox from "@/components/custom-inputs/SelectBox.vue";
import Notification from "@/models/api/Notification";
import PickerComponent from "@/components/custom-inputs/Picker.vue";
import {themeparkService} from "@/services/themepark.service";
import {AxiosResponse} from "axios";
import Destination from "@/models/api/Destination";
import {PushNotifications} from "@capacitor/push-notifications";

export default defineComponent({
  name: "NotificationsCreateView",
  components: {
    PickerComponent,
    SelectBox,
    NotificationComponent,
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
    Alert
  },
  computed: {
    ...mapState(['notificationsEnabled', 'settings', 'activeDestination', 'notificationHoldingArea']),
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

      if(!this.settings.noAds) {
        if(!this.adWatched) {
          waitTimeOptions = waitTimeOptions.filter(a => a.value >= 35);
        }
      }

      return waitTimeOptions;
    }
  },
  data() : { definitions: { criteria: Array<{ value: number, label: string }> }, adWatched: boolean, notification: Notification, destinations: Array<Destination> } {
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
      adWatched: false,
      notification: new Notification(),
      destinations: []
    }
  },
  methods: {

    async watchAd() {
      showRewardAdvertisement().then(() => {
        this.adWatched = true;
      });
    },

    requestNotificationPermissions() {
      // requestPermissions() {
      PushNotifications.requestPermissions().then((permissions) => {
        if(permissions.receive == 'granted') {
          setupOneSignal().then(() => {
            saveSubscriptionToDatabase().then(() => {
              this.$store.dispatch('setNotificationsEnabled', true);
            }).catch(() => {
              this.$store.dispatch('setNotificationsEnabled', false);
            });
          });
        }else{
          openAppNotificationSettings()
        }
      })
      // },
    },

    navigateToSubscriptions() {
      this.$router.push({
        name: 'notifications',
        params: {
          transition: 'slide-right'
        }
      })
    },

    backToWaitTimes() {
      this.$router.push({
        name: 'waitTimes',
        params: {
          transition: 'slide-left'
        }
      })

    },

    hideBannerAdvertisement() {
      this.$store.dispatch('setModalOpen', true);
      hideBannerAdvertisement();
    },

    resumeBannerAdvertisement() {
      this.$store.dispatch('setModalOpen', false);
      resumeBannerAdvertisement(this.settings.noAds);
    },


    createNotification() {
      let notificationToCreate = new CreateNotificationRequest({
        attractionId: this.notification.properties.attractionId,
        type: this.notification.properties.typeId,
        parkId: this.notification.properties.parkId,
        criteriaType: this.notification.properties.criteriaType,
        waitTime: this.notification.properties.waitTime
      });

      this.$store.dispatch('addNotification', notificationToCreate);

      this.navigateToSubscriptions();
    },

    setupNotification() {
      this.notification.attraction = this.notificationHoldingArea.attraction;
      this.notification.park = this.notificationHoldingArea.park;
      this.notification.properties.attractionId = this.notification.attraction.attractionId;
      this.notification.properties.parkId = this.notification.park.parkId;
      this.notification.properties.typeId = this.notificationHoldingArea.type;
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
  unmounted() {
    this.$store.commit('clearNotificationHoldingArea');
  },
  beforeMount() {
    this.getDestinations();
    this.setupNotification();
  }

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