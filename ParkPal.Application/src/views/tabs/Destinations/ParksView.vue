<template>
  <IonPage>
    <IonHeader :style="'background: ' + settings.theme.header.background + ' !important;'">
      <IonToolbar color="transparent" :style="'background: ' + settings.theme.header.background + ' !important;'">
        <IonButtons slot="start">
            <IonButton @click="backToDestinations">
              <FontAwesomeIcon icon="arrow-left" :color="settings.theme.header.icons" fixed-width></FontAwesomeIcon>
            </IonButton>
        </IonButtons>
        <IonTitle :style="'background: ' + settings.theme.header.background + ' !important; border-width: 0 0px 2px; border-style: solid;border-color: ' + settings.theme.header.border + ' !important; color: '+ settings.theme.header.text + ' !important;'">
          {{ activeDestination.name }}
        </IonTitle>
      </IonToolbar>
    </IonHeader>
    <IonContent :style="'background:' + settings.theme.background + '!important;'">
      <ConnectionError v-if="serverError" @retry="getParks"></ConnectionError>
      <Loader v-if="!parks.length && !serverError">Fetching Parks...</Loader>
      <ul class="parks" v-if="parks.filter(a => !a.hidden).length">
        <li v-for="park in parks.filter(a => !a.hidden)" :key="park.parkId" :style="'background: url(/img/' + park.image + ')'" @click="navigateToWaitTimes(park)">
          <div class="park-details">
            <p :class="{ 'no-premium' : !settings.parkPalPlus }">{{ parkName(park.name) }}</p>
            <div class="favourite" :class="{ 'favourite--active': isFavourite(park.parkId) }" v-if="settings.parkPalPlus">
              <FontAwesomeIcon icon="heart" size="2x" @click.stop="favouritePark(park.parkId)" fixed-width></FontAwesomeIcon>
            </div>
          </div>
        </li>
      </ul>
    </IonContent>
  </IonPage>

</template>

<style lang="scss" scoped>
.parks {
  list-style: none;
  padding: 0;
  margin: 0;

  li {
    list-style: none;
    margin: 0;
    padding: 0 16px;
    height: 150px;
    display: flex;
    align-items: center;
    justify-content: center;
    background-size: cover !important;
    background-position: center !important;
    color: #FFF;
    position: relative;

    .park-details {
      position: relative;
      z-index: 4;

      p {
        margin-top: 0;
        font-size: 24px;
        font-weight: 400;
      }

      .favourite {
        font-size: 12px;

        &.favourite--active {
          color: #FF4141;
        }
      }
    }

    &::before {
      content: '';
      position: absolute;
      top: 0;
      right: 0;
      bottom: 0;
      left: 0;
      background: rgba(0, 0, 0, 0.7);
      z-index: 3;
    }
  }
}

.no-premium {
  margin: 0;
}
</style>

<script lang="ts">
import {defineComponent} from "vue";
import { IonContent, IonPage, IonHeader, IonToolbar, IonTitle, IonButton, IonButtons } from "@ionic/vue";
import {mapGetters, mapState} from "vuex";
import {themeparkService} from "@/services/themepark.service";
import {AxiosResponse} from "axios";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import Destination from "@/models/api/Destination";
import Park from "@/models/api/Park";
import ConnectionError from "@/components/ConnectionError.vue";
import Loader from "@/components/Loader.vue";

export default defineComponent({
  name: "ParksView",
  components: {
    IonPage,
    IonContent,
    IonHeader,
    IonToolbar,
    IonTitle,
    IonButtons,
    IonButton,
    FontAwesomeIcon,
    ConnectionError,
    Loader
  },
  computed: {
    ...mapState(['destinations', 'activeDestination', 'settings', 'serverError']),
    ...mapGetters(['favourites'])
  },
  data(): { parks: Array<Park> } {
    return {
      parks: []
    }
  },
  beforeMount() {
      this.getParks();
  },
  methods: {
    isFavourite(parkId: string): boolean {
      return this.favourites.includes(parkId);
    },
    getParks() {
      themeparkService.getParks(this.activeDestination.destinationId).then(
          (response: AxiosResponse<Destination>) => {
            response.data.parks.forEach(park => {
              park = new Park(park);
              park.checkImageExists().then(exists => {
                if(exists) {
                  this.parks.push(park);
                }
              })
            })

            this.$store.dispatch('setServerError', false);
          })
          .catch(() => {
            this.$store.dispatch('setServerError', true);
          });
    },
    parkName(name: string) {
      return name.replace('Theme', '').replace('Park', '');
    },
    backToDestinations() {
      this.$router.push({
        name: 'destinations',
        params: {
          transition: 'slide-left'
        }
      })
    },
    navigateToWaitTimes(park: Park) {
      this.$store.dispatch('setActivePark', park);

      this.$router.push({
        name: 'waitTimes',
        params: {
          transition: 'slide-right'
        }
      })
    },
    favouritePark(id: string) {
      if (this.favourites.includes(id)) {
        this.$store.dispatch('removeFavourite', id);
      } else {
        this.$store.dispatch('addFavourite', id);
      }
    },
  },
})
</script>