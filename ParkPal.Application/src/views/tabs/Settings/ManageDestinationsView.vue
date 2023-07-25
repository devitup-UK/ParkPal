<template>
  <IonPage>
    <IonHeader :style="'background: ' + settings.theme.header.background + ' !important;'">
      <IonToolbar color="transparent" :style="'background: ' + settings.theme.header.background + ' !important;'">
        <IonButtons slot="start">
          <IonButton @click="backToSettings">
            <FontAwesomeIcon icon="arrow-left" :color="settings.theme.header.icons" fixed-width></FontAwesomeIcon>
          </IonButton>
        </IonButtons>
        <IonTitle :style="'background: ' + settings.theme.header.background + ' !important; border-width: 0 0px 2px; border-style: solid;border-color: ' + settings.theme.header.border + ' !important; color: '+ settings.theme.header.text + ' !important;'">Manage Destinations</IonTitle>
        <IonButtons slot="end">
          <IonButton @click="displaySortSheet">
            <FontAwesomeIcon icon="arrow-down-wide-short" :color="settings.theme.header.icons" fixed-width></FontAwesomeIcon>
          </IonButton>
        </IonButtons>
      </IonToolbar>
    </IonHeader>
    <IonContent :style="`background:${settings.theme.background} !important;`">
      <Loader v-if="!destinations.length">Fetching Destinations...</Loader>
      <template v-else>
        <IonRow>
          <IonCol class="enable-all-button">
            <IonButton expand="block" @click="enableAllDestinations" :style="`--background: ${settings.theme.actionButtonBackground}; --color: ${settings.theme.actionButtonText};`">Enable All Destinations</IonButton>
          </IonCol>
        </IonRow>
        <IonRow>
          <IonCol class="disable-all-button">
            <IonButton expand="block" @click="disableAllDestinations" :style="`--background: ${settings.theme.actionButtonBackground}; --color: ${settings.theme.actionButtonText};`">Disable All Destinations</IonButton>
          </IonCol>
        </IonRow>
        <IonList lines="full" class="settings-list" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonItem v-for="destination in destinations" :key="destination.destinationId" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
            <IonLabel>{{ destination.name }}</IonLabel>
            <IonToggle color="success" v-model="destination.visible" @click="toggleDestination(destination.destinationId)"></IonToggle>
          </IonItem>
        </IonList>
      </template>
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
  border-top: 1px solid var(--ion-item-border-color, var(--ion-border-color, var(--ion-color-step-250, #c8c7cc)));;
}

.settings-icon {
  font-size: 20px;
  margin-right: 5px;
}

.disable-all-button {
  padding-top: 0;
}

.enable-all-button, .disable-all-button {
  ion-button {
    margin: 0;
  }
}
</style>

<script lang="ts">
import {defineComponent} from "vue";
import {mapState} from "vuex";
import {
  actionSheetController,
  IonButton,
  IonButtons,
  IonCol,
  IonContent,
  IonHeader,
  IonItem,
  IonLabel,
  IonList,
  IonPage,
  IonRow,
  IonTitle,
  IonToggle,
  IonToolbar
} from "@ionic/vue";
import {FontAwesomeIcon} from "@fortawesome/vue-fontawesome";
import {themeparkService} from "@/services/themepark.service";
import {AxiosResponse} from "axios";
import Destination from "@/models/api/Destination";
import Loader from "@/components/Loader.vue";
import {App} from "@capacitor/app";
import {DestinationSort} from "@/models/enums/DestinationSort";
import sortArray from "sort-array";
import store from "@/store";
import {
  hideBannerAdvertisement,
  resumeBannerAdvertisement,
  showBannerAdvertisement
} from "@/handlers/advertisements.handler";

export default defineComponent({
  name: "SettingsManageDestinationsView",
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
    IonToggle,
    IonLabel,
    FontAwesomeIcon,
    IonRow,
    IonCol,
    Loader
  },
  computed: {
    ...mapState(['settings', 'settings'])
  },
  data(): { destinations: Array<Destination>, sort: DestinationSort } {
    return {
      destinations: [],
      sort: DestinationSort.Default
    }
  },
  beforeMount() {
    this.getDestinations();

    App.addListener('resume',() => {
      if(this.$route.name == 'settingsManageDestinations') {
        this.getDestinations();
      }
    })


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
    },

    toggleDestination(destinationId: string) {
      this.$store.dispatch('toggleDestination', destinationId);
    },

    enableAllDestinations() {
      this.$store.dispatch('enableAllDestinations').then(() => {
        this.markDestinationsAsHidden(this.destinations);
      });
    },

    disableAllDestinations() {
      this.$store.dispatch('disableAllDestinations', this.destinations).then(() => {
        this.markDestinationsAsHidden(this.destinations);
      });
    },

    getDestinations() {
      // Get all the destinations first and mark them as hidden.
      themeparkService.getDestinations().then((response: AxiosResponse<Array<Destination>>) => {
        this.markDestinationsAsHidden(response.data);
      })
    },

    markDestinationsAsHidden(destinations: Array<Destination>) {
      let defaultOrder = 1;

      destinations.forEach(destination => {
        let transformedDestination = new Destination(destination);

        let targetDestination = this.destinations.find(a => a.destinationId == destination.destinationId);

        if(this.settings.hiddenDestinations.includes(transformedDestination.destinationId)) {
          if(targetDestination) {
              targetDestination.hidden = true;
              targetDestination.visible = false;
          }else{
            transformedDestination.hidden = true;
            transformedDestination.visible = false;
          }
        }else{
          if(targetDestination) {
            targetDestination.hidden = false;
            targetDestination.visible = true;
          }else{
            transformedDestination.hidden = false;
            transformedDestination.visible = true;
          }
        }

        if(!targetDestination) {
          transformedDestination.defaultOrder = defaultOrder;
          this.destinations.push(transformedDestination);
        }

        defaultOrder++;

      })

      this.sortDestinations();
    },

    sortDestinations() {
      switch(this.sort) {
        case DestinationSort.Alphabetical:
          this.destinations = sortArray(this.destinations, {
            by: "name",
            order: "asc"
          });
          break;
        case DestinationSort.ReverseAlphabetical:
          this.destinations = sortArray(this.destinations, {
            by: "name",
            order: "desc"
          });
          break;
        case DestinationSort.Default:
          this.destinations = sortArray(this.destinations, {
            by: "defaultOrder",
            order: "asc"
          });
          break;
      }
    },

    async displaySortSheet() {
      const presentActionSheet = async () => {
        const actionSheet = await actionSheetController.create({
          header: "Sort Destinations",
          buttons: [
            {
              text: "Default",
              handler: () => {
                this.sort = DestinationSort.Default;
                this.sortDestinations();
              }
            },
            {
              text: "A-Z",
              handler: () => {
                this.sort = DestinationSort.Alphabetical;
                this.sortDestinations();
              }
            },
            {
              text: "Z-A",
              handler: () => {
                this.sort = DestinationSort.ReverseAlphabetical;
                this.sortDestinations();
              }
            },
          ]
        });

        actionSheet.onDidDismiss().then(() => {
          resumeBannerAdvertisement(this.settings.noAds);
        })

        hideBannerAdvertisement();

        await actionSheet.present();

      }

      await presentActionSheet();
    }
  }
})
</script>