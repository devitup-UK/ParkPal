<template>
  <IonPage>
    <IonHeader :style="'background: ' + settings.theme.header.background + ' !important;'">
      <IonToolbar color="transparent" :style="'background: ' + settings.theme.header.background + ' !important;'">
        <IonButtons slot="start">
          <IonButton @click="backToWaitTimes()">
            <FontAwesomeIcon icon="arrow-left" :color="settings.theme.header.icons" fixed-width></FontAwesomeIcon>
          </IonButton>
        </IonButtons>x
        <IonTitle :style="'background: ' + settings.theme.header.background + ' !important; border-width: 0 0px 2px; border-style: solid;border-color: ' + settings.theme.header.border + ' !important; color: '+ settings.theme.header.text + ' !important;'">Wait Time Filters</IonTitle>
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
              <h3 :style="`color: ${settings.theme.selectionBoxText} !important; background: transparent;`">Sort</h3>
            </IonCol>
            <IonCol cols="6" class="select-filter__input">
                <IonSelect interface="action-sheet" cancelText="Cancel" v-model="filters.sort" @ionFocus="hideAdvertisement" @ionDismiss="resumeAdvertisement">
                  <IonSelectOption v-for="sort in definitions.sort" :key="sort.value" :value="sort.value">{{ sort.label }}</IonSelectOption>
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
              <IonButton expand="full" @click="resetFilter" color="transparent" :style="`color: ${settings.theme.resetButtonText} !important; background: ${settings.theme.resetButtonBackground} !important;`">
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
import WaitTimeFilter from "@/models/store/WaitTimeFilter";
import {mapState} from "vuex";
import {hideBannerAdvertisement, showBannerAdvertisement} from "@/handlers/advertisements.handler";

export default defineComponent({
  name: "WaitTimesFilterView",
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
    ...mapState(['settings'])
  },
  data() {
    return {
      definitions: {
        type: [
          {
            value: 0,
            label: 'All Attractions'
          },
          {
            value: 1,
            label: 'Favourites'
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
        ]
      },
      filters: new WaitTimeFilter()
    }
  },
  beforeMount() {
    this.filters = this.$store.state.filters.waitTimeFilter;
  },
  methods: {
    applyFilter() {
      this.$store.commit('setWaitTimeFilter', this.filters);

      this.$router.push({
        name: 'waitTimes',
        params: {
          transition: 'slide-left'
        }
      })
    },

    resetFilter() {
      this.filters = new WaitTimeFilter()
    },

    backToWaitTimes() {
      this.$router.push({
        name: 'waitTimes',
        params: {
          transition: 'slide-left'
        }
      })
    },

    hideAdvertisement() {
      this.$store.dispatch('setModalOpen', true);
      hideBannerAdvertisement();
    },

    resumeAdvertisement() {
      this.$store.dispatch('setModalOpen', false);
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