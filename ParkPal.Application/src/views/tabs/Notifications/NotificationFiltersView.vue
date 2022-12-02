<template>
  <IonPage>
    <IonHeader :style="'background: ' + settings.theme.header.background + ' !important;'">
      <IonToolbar color="transparent" :style="'background: ' + settings.theme.header.background + ' !important;'">
        <IonButtons slot="start">
          <IonButton @click="backToNotifications()">
            <FontAwesomeIcon icon="arrow-left" :color="settings.theme.header.icons" fixed-width></FontAwesomeIcon>
          </IonButton>
        </IonButtons>
        <IonTitle :style="'background: ' + settings.theme.header.background + ' !important; border-width: 0 0px 2px; border-style: solid;border-color: ' + settings.theme.header.border + ' !important; color: '+ settings.theme.header.text + ' !important;'">Notification Filters</IonTitle>
      </IonToolbar>
    </IonHeader>
    <IonContent :style="`background:${settings.theme.background} !important;`">
      <IonGrid>
        <form class="filter-form">
          <IonRow class="select-filter" :style="`background:${settings.theme.selectionBoxBackground} !important; border-color: ${settings.theme.selectionBoxBorder} !important;`">
            <IonCol cols="6" class="select-filter__label">
              <h3 :style="`color: ${settings.theme.selectionBoxText} !important; background: transparent;`">Type</h3>
            </IonCol>
            <IonCol cols="6" class="select-filter__input">
              <IonSelect interface="action-sheet" cancelText="Cancel" v-model="filters.type" @ionFocus="hideAdvertisement" @ionDismiss="resumeAdvertisement">
                <IonSelectOption v-for="type in definitions.type" :key="type.value" :value="type.value">{{ type.label }}</IonSelectOption>
              </IonSelect>
            </IonCol>
          </IonRow>
          <IonRow class="select-filter" :style="`background:${settings.theme.selectionBoxBackground} !important; border-color: ${settings.theme.selectionBoxBorder} !important;`">
            <IonCol cols="6" class="select-filter__label">
              <h3 :style="`color: ${settings.theme.selectionBoxText} !important; background: transparent;`">Criteria</h3>
            </IonCol>
            <IonCol cols="6" class="select-filter__input">
              <IonSelect interface="action-sheet" cancelText="Cancel" v-model="filters.criteria" @ionFocus="hideAdvertisement" @ionDismiss="resumeAdvertisement">
                <IonSelectOption v-for="criteria in definitions.criteria" :key="criteria.value" :value="criteria.value">{{ criteria.label }}</IonSelectOption>
              </IonSelect>
            </IonCol>
          </IonRow>
          <IonRow class="select-filter" :style="`background:${settings.theme.selectionBoxBackground} !important; border-color: ${settings.theme.selectionBoxBorder} !important;`">
            <IonCol cols="6" class="select-filter__label">
              <h3 :style="`color: ${settings.theme.selectionBoxText} !important; background: transparent;`">Sort</h3>
            </IonCol>
            <IonCol cols="6" class="select-filter__input">
              <IonSelect interface="action-sheet" cancelText="Cancel" v-model="filters.sort" @ionFocus="hideAdvertisement" @ionDismiss="resumeAdvertisement">
                <IonSelectOption v-for="sort in definitions.sort" :key="sort.value" :value="sort.value">{{ sort.label }}</IonSelectOption>
              </IonSelect>
            </IonCol>
          </IonRow>
          <IonRow class="select-filter" :style="`background:${settings.theme.selectionBoxBackground} !important; border-color: ${settings.theme.selectionBoxBorder} !important;`">
            <IonCol cols="6" class="select-filter__label">
              <h3 :style="`color: ${settings.theme.selectionBoxText} !important; background: transparent;`">Park</h3>
            </IonCol>
            <IonCol cols="6" class="select-filter__input">
              <IonSelect interface="action-sheet" cancelText="Cancel" v-model="filters.parkId" @ionFocus="hideAdvertisement" @ionDismiss="resumeAdvertisement">
                <IonSelectOption v-for="park in definitions.parks" :key="park.value" :value="park.value">{{ park.label }}</IonSelectOption>
              </IonSelect>
            </IonCol>
          </IonRow>
          <IonRow class="filter-button">
            <IonCol>
              <IonButton expand="full" @click="applyFilter" color="transparent" :style="`color: ${settings.theme.actionButtonText} !important; background: ${settings.theme.actionButtonBackground} !important;`">
                APPLY FILTER
              </IonButton>
            </IonCol>
          </IonRow>
          <IonRow class="filter-button">
            <IonCol>
              <IonButton expand="full" @click="resetFilter"  color="transparent" :style="`color: ${settings.theme.resetButtonText} !important; background: ${settings.theme.resetButtonBackground} !important;`">
                RESET FILTER
              </IonButton>
            </IonCol>
          </IonRow>
        </form>
      </IonGrid>
    </IonContent>
  </IonPage>
</template>

<script>
import {defineComponent} from "vue";
import { IonToolbar, IonPage, IonContent, IonButtons, IonButton, IonTitle, IonHeader, IonSelect, IonSelectOption, IonGrid, IonRow, IonCol } from "@ionic/vue";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import {mapState} from "vuex";
import NotificationsFilter from "@/models/store/NotificationsFilter";
import {hideBannerAdvertisement, showBannerAdvertisement} from "@/handlers/advertisements.handler";

export default defineComponent({
  name: "NotificationFiltersView",
  components: {
    FontAwesomeIcon,
    IonPage,
    IonContent,
    IonToolbar,
    IonButtons,
    IonButton,
    IonTitle,
    IonHeader,
    IonGrid,
    IonRow,
    IonCol,
    IonSelect,
    IonSelectOption
  },
  computed: {
    ...mapState(['notifications', 'settings'])
  },
  data() {
    return {
      definitions: {
        type: [
          {
            value: 0,
            label: 'All'
          },
          {
            value: 1,
            label: 'Attractions'
          },
          {
            value: 2,
            label: 'Parks'
          },
          {
            value: 3,
            label: 'Favourites'
          }
        ],
        criteria: [
          {
            value: 0,
            label: 'Any'
          },
          {
            value: 1,
            label: 'Less Than'
          },
          {
            value: 2,
            label: 'More Than'
          },
          {
            value: 3,
            label: 'Equal To'
          }
        ],
        sort: [
          {
            value: 0,
            label: 'Lowest Wait Time'
          },
          {
            value: 1,
            label: 'Highest Wait Time'
          },
          {
            value: 2,
            label: 'Thrill Rides'
          },
          {
            value: 3,
            label: 'Tame Rides'
          }
        ],
        parks: [
          {
            value: null,
            label: 'Any'
          }
        ]
      },
      filters: new NotificationsFilter()
    }
  },
  beforeMount() {
    this.filters = this.$store.state.filters.notificationsFilter;
    this.notifications.forEach((notification) => {
      if(!this.definitions.parks.filter(a => a.value == notification.park.parkId).length) {
        this.definitions.parks.push({
          value: notification.park?.parkId,
          label: notification.park?.name
        })
      }
    })
  },
  methods: {
    applyFilter() {
      this.$store.commit('setNotificationsFilter', this.filters);

      this.$router.push({
        name: 'notifications',
        params: {
          transition: 'slide-left'
        }
      })
    },

    resetFilter() {
      this.filters = new NotificationsFilter()
    },

    backToNotifications() {
      this.$router.push({
        name: 'notifications',
        params: {
          transition: 'slide-left'
        }
      })
    },

    hideAdvertisement() {
      hideBannerAdvertisement();
    },

    resumeAdvertisement() {
      showBannerAdvertisement(this.settings.parkPalPlus);
    }
  }
});
</script>

<style lang="scss" scoped>
.select-filter {
  border-width: 0 0 1px;
  border-style: solid;
  border-color: #D5D5D5;

  &:nth-child(1) {
    border-width: 1px 0;
  }

  &:nth-child(4) {
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

.filter-form {
  margin-top: 20px;
}

ion-select {
  text-align: right;

  &::part(icon) {
    display: none !important;
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
</style>