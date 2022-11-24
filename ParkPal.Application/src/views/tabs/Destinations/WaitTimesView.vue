<template>
  <IonPage>
    <IonHeader :style="'background: ' + settings.theme.header.background + ' !important;'">
      <IonToolbar color="transparent" :style="'background: ' + settings.theme.header.background + ' !important;'">
        <IonButtons slot="start">
          <IonButton @click="backToParks">
            <FontAwesomeIcon icon="arrow-left" :color="settings.theme.header.icons" fixed-width></FontAwesomeIcon>
          </IonButton>
        </IonButtons>
        <IonTitle :style="'background: ' + settings.theme.header.background + ' !important; border-width: 0 0px 2px; border-style: solid;border-color: ' + settings.theme.header.border + ' !important; color: '+ settings.theme.header.text + ' !important;'">{{ activePark.name }}</IonTitle>
        <IonButtons slot="end">
          <IonButton @click="filterAttractions">
            <FontAwesomeIcon icon="filter" :color="settings.theme.header.icons" fixed-width></FontAwesomeIcon>
          </IonButton>
        </IonButtons>
      </IonToolbar>
    </IonHeader>
    <IonContent :style="`background:${settings.theme.background} !important;`">
      <IonRefresher slot="fixed" @ionRefresh="getAttractions($event)">
        <IonRefresherContent pullingText="Refresh Wait Times" refreshingText="Fetching Wait Times..."></IonRefresherContent>
      </IonRefresher>
      <ConnectionError v-if="serverError" @retry="getAttractions"></ConnectionError>
      <Loader v-if="loading">Fetching Wait Times...</Loader>
      <template v-else>
        <IonSearchbar placeholder="Search" debounce="600" @ionChange="searchInAttractions" v-model="waitTimeSearch" :style="`--background: ${settings.theme.searchBoxBackground}; --color: ${settings.theme.searchBoxText}; --icon-color: ${settings.theme.searchBoxIcons}; --clear-button-color: ${settings.theme.searchBoxIcons};`"></IonSearchbar>
        <template v-if="attractions.filter(a => !a.hidden).length">
          <ul class="attractions" v-if="!waitTimeSearch.length">
            <AttractionComponent v-for="attraction in attractions.filter(a => !a.hidden)" :key="attraction.attractionId" :attraction="attraction" :park="activePark" :notification="notifications.filter(a => a.attraction.attractionId == attraction.attractionId).length ? notifications.filter(a => a.attraction.attractionId == attraction.attractionId).length[0] : null"></AttractionComponent>
          </ul>
          <ul class="attractions" v-if="waitTimeSearch.length && searchAttractions.filter(a => !a.hidden).length">
            <AttractionComponent v-for="attraction in searchAttractions.filter(a => !a.hidden)" :key="attraction.attractionId" :attraction="attraction" :park="activePark" :notification="notifications.filter(a => a.attraction.attractionId == attraction.attractionId).length ? notifications.filter(a => a.attraction.attractionId == attraction.attractionId).length[0] : null"></AttractionComponent>
          </ul>
          <div class="no-wait-times" v-if="waitTimeSearch.length && !searchAttractions.filter(a => !a.hidden).length">
            <div class="no-wait-times__image">
              <img src="@/assets/no-wait-times.svg">
            </div>
            <p :style="'color: ' + settings.theme.text + ' !important;'">There are no attractions that match your search criteria, please change your search term and try again.</p>
            <div class="filter-button">
              <IonButton expand="full" @click="waitTimeSearch = ''" color="transparent" :style="`color: ${settings.theme.resetButtonText} !important; background: ${settings.theme.resetButtonBackground} !important;`">
                RESET SEARCH
              </IonButton>
            </div>
          </div>
        </template>
        <div class="no-wait-times" v-else>
          <div class="no-wait-times__image">
            <img src="@/assets/no-wait-times.svg">
          </div>
          <p :style="'color: ' + settings.theme.text + ' !important;'">There are no attractions that match your filter criteria, please change your filters and try again.</p>
          <div class="filter-button">
            <IonButton expand="full" @click="resetFilters" color="transparent" :style="`color: ${settings.theme.resetButtonText} !important; background: ${settings.theme.resetButtonBackground} !important;`">
              RESET FILTERS
            </IonButton>
          </div>
        </div>
      </template>
    </IonContent>
  </IonPage>

</template>

<style lang="scss" scoped>
.attractions {
  list-style: none;
  padding: 0;
  margin: 0;
}

.no-wait-times {
  display: flex;
  height: 100%;
  align-items: center;
  justify-content: center;
  flex-direction: column;

  p {
    font-size: 14px;
    margin: 0 50px 14px;
  }
}

.filter-button {
  width: 100%;

    ion-button {
      margin: 0;
      font-weight: 300;
      font-size: 14px;
    }
  }

.action-sheet-title {
  border-bottom: 1px solid #3F3F3F;
}
</style>

<style lang="scss">
.refresher-pulling-text, .refresher-refreshing-text {
  text-transform: uppercase !important;
  font-size: 12px !important;
  color: #3f3f3f;
}

.searchbar-input {
  text-align: left !important;
}
</style>

<script lang="ts">
import {defineComponent} from "vue";
import {
  IonContent,
  IonPage,
  IonHeader,
  IonToolbar,
  IonTitle,
  IonButton,
  IonButtons,
  IonRefresher,
  IonRefresherContent, RefresherCustomEvent,
  IonSearchbar
} from "@ionic/vue";
import {mapGetters, mapState} from "vuex";
import {themeparkService} from "@/services/themepark.service";
import {AxiosError, AxiosResponse} from "axios";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import WaitTimeFilter from "@/models/store/WaitTimeFilter";
import AttractionComponent from "@/components/Attraction.vue";
import Park from "@/models/api/Park";
import Loader from "@/components/Loader.vue";
import Attraction from "@/models/api/Attraction";
import ConnectionError from "@/components/ConnectionError.vue";

export default defineComponent({
  name: "WaitTimesView",
  components: {
    AttractionComponent,
    IonPage,
    IonContent,
    IonHeader,
    IonToolbar,
    IonTitle,
    IonButtons,
    IonButton,
    FontAwesomeIcon,
    Loader,
    IonRefresher,
    IonRefresherContent,
    ConnectionError,
    IonSearchbar
  },
  computed: {
    ...mapState(['destinations', 'filters', 'activePark', 'activeDestination', 'notifications', 'settings', 'serverError']),
    ...mapGetters(['favourites', 'notificationIds'])
  },
  data(): { attractions: Array<Attraction>, searchAttractions: Array<Attraction>, loading: boolean, waitTimeSearch: string  } {
    return {
      attractions: [],
      searchAttractions: [],
      loading: true,
      waitTimeSearch: ''
    }
  },
  beforeMount() {
    this.$store.dispatch('getAllNotifications', {
      filters: this.filters.notificationsFilter,
      favouriteAttractionIds: this.favourites
    });

    this.getAttractions(null);
  },
  methods: {
    getAttractions(event: RefresherCustomEvent | null) {
      this.$store.dispatch('setServerError', false);
      themeparkService.getAttractions(this.activePark.parkId, {
        filters: this.filters.waitTimeFilter,
        favouriteAttractionIds: this.favourites
      }).then((response: AxiosResponse<Park>) => {
        this.attractions = response.data.attractions;

        response.data.attractions.forEach(attraction => {
          let transformedAttraction = new Attraction(attraction);

          transformedAttraction.checkImageExists().then(exists => {
            if (!exists) {
              this.attractions = this.attractions.filter(a => a.attractionId != transformedAttraction.attractionId);
            }
          })
        })

        this.loading = false;
        event?.target?.complete();
      }).catch(() => {
            this.$store.dispatch('setServerError', true);
          });
    },
    // Not implemented yet.
    backToParks() {
      if(this.activeDestination.parks.length > 1) {
        this.$router.push({
          name: 'parks',
          params: {
            transition: 'slide-left'
          }
        })
      }else{
        this.$router.push({
          name: 'destinations',
          params: {
            transition: 'slide-left'
          }
        })
      }
    },
    searchInAttractions() {
      this.searchAttractions = this.attractions.filter(a => a.name?.toLowerCase().includes(this.waitTimeSearch.toLowerCase()));
    },

    filterAttractions() {
      this.$router.push({
        name: 'waitTimeFilters',
        params: {
          transition: 'slide-right'
        }
      })
    },

    resetFilters() {
      this.$store.commit('setWaitTimeFilter', new WaitTimeFilter());
      this.getAttractions(null);
    },


  }
})
</script>