<template>
  <IonPage>
    <IonHeader :style="'background: ' + settings.theme.header.background + ' !important;'">
      <IonToolbar color="transparent" :style="'background: ' + settings.theme.header.background + ' !important;'">
        <IonTitle :style="'background: ' + settings.theme.header.background + ' !important; color: '+ settings.theme.header.text + ' !important;'">
          Destinations
        </IonTitle>
        <IonButtons slot="end">
          <IonButton @click="filterDestinations">
            <FontAwesomeIcon icon="arrow-down-wide-short" :color="settings.theme.header.icons" fixed-width></FontAwesomeIcon>
          </IonButton>
        </IonButtons>
      </IonToolbar>
      <IonSearchbar placeholder="Search" v-model="searchTerm" @ionChange="search" @keyup.enter="dismissKeyboard" debounce="400" :style="`--background: ${settings.theme.searchBoxBackground}; --color: ${settings.theme.searchBoxText}; --icon-color: ${settings.theme.searchBoxIcons}; --clear-button-color: ${settings.theme.searchBoxIcons};`" class="destination-searchbar"></IonSearchbar>
    </IonHeader>
    <IonContent :style="'background:' + settings.theme.background + '!important;'">
      <ConnectionError v-if="serverError" @retry="getDestinations"></ConnectionError>
      <Loader v-if="loading && !serverError">Fetching Destinations...</Loader>
      <Swiper @swiper="onSwiper" :modules="[Virtual]" :virtual="true" v-show="!loading && !serverError && (!searchDestinations.length && !searchTerm.length) && destinations.length !== destinations.filter(a => a.hidden).length" :slides-per-view="1" @slideChange="slideChanged" :initial-slide="destinationSlideIndex" observer>
        <SwiperSlide v-for="(destination, index) in destinations.filter(a => !a.hidden)" :key="destination.destinationId" :virtualIndex="index">
          <DestinationComponent :destination="destination" />
        </SwiperSlide>
      </Swiper>
      <div class="no-destinations-from-search" v-if="!loading && destinations.length === destinations.filter(a => a.hidden).length && (!searchDestinations.length && !searchTerm.length)">
        <p></p>
        <div class="no-destinations-from-search__image">
          <img src="@/assets/no-wait-times.svg">
        </div>
        <p :style="'color: ' + settings.theme.text + ' !important;'">You have hidden all destinations, to see your destinations again, head to the "Manage Destinations" section under Settings or use the button below.</p>
        <div class="filter-button">
          <IonButton expand="full" @click="navigateToManageDestinations" color="transparent" :style="`color: ${settings.theme.resetButtonText} !important; background: ${settings.theme.resetButtonBackground} !important;`">
            MANAGE DESTINATIONS
          </IonButton>
        </div>
      </div>
      <ul class="mini-destinations" v-if="!loading && searchDestinations.length">
         <MiniDestination @click="navigateToParksOrAttractions(destination)" v-for="(destination) in searchDestinations.filter(a => !a.hidden)" :destination="destination" :key="'mini-' + destination.destinationId" />
      </ul>
      <div class="no-destinations-from-search" v-if="!loading && (!searchDestinations.length || (searchDestinations.length && searchDestinations.length === searchDestinations.filter(a => a.hidden).length)) && searchTerm.length">
        <div class="no-destinations-from-search__image">
          <img src="@/assets/no-wait-times.svg">
        </div>
        <p :style="'color: ' + settings.theme.text + ' !important;'">There are no destinations that match your search criteria, please change your search term and try again.</p>
        <div class="filter-button">
          <IonButton expand="full" @click="searchTerm = ''" color="transparent" :style="`color: ${settings.theme.resetButtonText} !important; background: ${settings.theme.resetButtonBackground} !important;`">
            RESET SEARCH
          </IonButton>
        </div>
      </div>
    </IonContent>
  </IonPage>
</template>

<script lang="ts">
import {defineComponent} from "vue";
import {
  IonContent,
  IonPage,
  IonHeader,
  IonTitle,
  IonToolbar,
  IonSearchbar,
  IonButton,
  IonButtons,
  actionSheetController
} from "@ionic/vue";
import DestinationComponent from "@/components/Destination.vue";
import {mapState} from "vuex";
import Loader from "@/components/Loader.vue";
import ConnectionError from "@/components/ConnectionError.vue";
import { Swiper, SwiperSlide } from "swiper/vue";
import { Virtual } from 'swiper';
import Destination from "@/models/api/Destination";
import MiniDestination from "@/components/MiniDestination.vue";


import 'swiper/scss';
import '@ionic/vue/css/ionic-swiper.css';
import {themeparkService} from "@/services/themepark.service";
import {AxiosResponse} from "axios";
import {SwiperModule} from "swiper/types";
import { Keyboard } from "@capacitor/keyboard";
import {FontAwesomeIcon} from "@fortawesome/vue-fontawesome";
import store from "@/store";
import {hideBannerAdvertisement} from "@/handlers/advertisements.handler";
import {deleteNotification} from "@/handlers/modals.handler";
import {DestinationSort} from "@/models/enums/DestinationSort";
import sortArray from "sort-array";

export default defineComponent({
  name: "IndexView",
  components: {
    ConnectionError,
    Swiper,
    SwiperSlide,
    DestinationComponent,
    IonPage,
    IonContent,
    Loader,
    IonToolbar,
    IonHeader,
    IonTitle,
    IonSearchbar,
    MiniDestination,
    IonButton,
    IonButtons,
    FontAwesomeIcon
  },
  computed: {
    ...mapState(['settings', 'serverError', 'destinationSlideIndex', 'destinationSearchTerm']),
  },
  data(): { Virtual: SwiperModule, destinations: Array<Destination>, swiper: typeof Swiper | null, loading: boolean, searchTerm: string, searchDestinations: Array<Destination>, sort: DestinationSort } {
    return {
      Virtual,
      destinations: [],
      swiper: null,
      loading: true,
      searchTerm: '',
      searchDestinations: [],
      sort: DestinationSort.Default
    }
  },
  watch: {
    destinationSearchTerm(value) {
      this.runSearch(value);
    }
  },
  beforeMount() {
    this.searchTerm = this.destinationSearchTerm;
    this.getDestinations();
  },
  methods: {
    dismissKeyboard() {
      Keyboard.hide();
    },
    runSearch(value: string) {
      if(value.length) {
        this.searchDestinations = this.destinations.filter(a => JSON.stringify(a).toLowerCase().includes(value.toLowerCase()));
      }else{
        this.searchDestinations = [];
      }
    },
    slideChanged() {
      if(!this.loading) {
        this.$store.dispatch('setDestinationSlideIndex', this.swiper?.activeIndex);
      }
    },
    search() {
      this.$store.dispatch('setDestinationSearchTerm', this.searchTerm);
    },
    onSwiper(swiper: typeof Swiper) {
      this.swiper = swiper;
    },
    getDestinations() {
      this.$store.dispatch('setServerError', false);

      // Get all the destinations first and mark them as hidden.
      themeparkService.getDestinations().then((response: AxiosResponse<Array<Destination>>) => {

        let destinationStored = new Promise((resolve) => {
          let defaultOrder = 1;


          response.data.forEach(destination => {
            let transformedDestination = new Destination(destination);
            transformedDestination.defaultOrder = defaultOrder;

            if (this.settings.hiddenDestinations.includes(transformedDestination.destinationId)) {
              transformedDestination.hidden = true;
            }

            this.destinations.push(transformedDestination);

            if(this.destinations.length == response.data.length) {
              resolve(true);
            }

            defaultOrder++;
          })
        });

        destinationStored.then(() => {
          this.swiper?.slideTo(this.destinationSlideIndex);
          setTimeout(() => {
            this.loading = false;
            this.runSearch(this.searchTerm);
          }, 300);
        })


      }).catch(() => {
        this.$store.dispatch('setServerError', true);
      })
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
    async filterDestinations() {
        const actionSheet = await actionSheetController.create({
          header: 'Sort Destinations',
          buttons: [
            {
              text: 'Default',
              handler: async () => {
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

      hideBannerAdvertisement();

      await actionSheet.present();
    },
    navigateToParksOrAttractions(destination: Destination) {
      this.$store.dispatch('setActiveDestination', destination);

      if(destination.parks?.length > 1) {
        this.$router.push({
          name: 'parks',
          params: {
            transition: 'slide-right'
          }
        });
      }else{
        this.$store.dispatch('setActivePark', destination.parks[0]);

        this.$router.push({
          name: 'waitTimes',
          params: {
            transition: 'slide-right'
          }
        });
      }
    },
    navigateToManageDestinations() {
      this.$router.push({
        name: 'settingsManageDestinations',
        params: {
          transition: 'slide-right'
        }
      })
    }
  }
})
</script>

<style lang="scss" scoped>
.swiper {
  height: 100%;
}

.destination-searchbar {
  padding-top: unset;
  //padding-bottom: unset;
  height: 50px;
}

.mini-destinations {
  list-style: none;
  padding: 0;
  margin: 0;
}

.no-destinations-from-search {
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
</style>