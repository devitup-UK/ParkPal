<template>
  <IonPage>
    <IonContent scroll-y="false" :style="'background:' + settings.theme.background + '!important;'">
      <ConnectionError v-if="serverError" @retry="getDestinations"></ConnectionError>
      <Loader v-if="!destinations.length && !serverError">Fetching Destinations...</Loader>
      <Swiper :modules="[Virtual]" virtual>
        <SwiperSlide v-for="(destination, index) in destinations.filter(a => !a.hidden)" :key="destination.destinationId" :virtualIndex="index">
          <DestinationComponent :destination="destination" />
        </SwiperSlide>
      </Swiper>
    </IonContent>
  </IonPage>
</template>

<script lang="ts">
import {defineComponent} from "vue";
import {IonContent, IonPage} from "@ionic/vue";
import DestinationComponent from "@/components/Destination.vue";
import {mapState} from "vuex";
import Loader from "@/components/Loader.vue";
import ConnectionError from "@/components/ConnectionError.vue";
import { Swiper, SwiperSlide } from "swiper/vue";
import { Virtual } from 'swiper';
import Destination from "@/models/api/Destination";


import 'swiper/scss';
import '@ionic/vue/css/ionic-swiper.css';
import {themeparkService} from "@/services/themepark.service";
import {AxiosResponse} from "axios";
import {SwiperModule} from "swiper/types";

export default defineComponent({
  name: "IndexView",
  components: {
    ConnectionError,
    Swiper,
    SwiperSlide,
    DestinationComponent,
    IonPage,
    IonContent,
    Loader
  },
  computed: {
    ...mapState(['settings', 'serverError']),
  },
  data(): { Virtual: SwiperModule, destinations: Array<Destination> } {
    return {
      Virtual,
      destinations: []
    }
  },
  beforeMount() {
    this.getDestinations();
  },
  methods: {
    getDestinations() {
      this.$store.dispatch('setServerError', false);

      // Get all the destinations first and mark them as hidden.
      themeparkService.getDestinations().then((response: AxiosResponse<Array<Destination>>) => {
        response.data.forEach(destination => {
          let transformedDestination = new Destination(destination);

          if(this.settings.hiddenDestinations.includes(transformedDestination.destinationId)) {
            transformedDestination.hidden = true;
          }

          transformedDestination.checkImageExists().then(exists => {
            if(exists) {
              console.log('An image exists', transformedDestination);
              this.destinations.push(transformedDestination);
            }
          })
        })
      }).catch(() => {
        this.$store.dispatch('setServerError', true);
      })
    }
  }
})
</script>

<style lang="scss" scoped>
.swiper {
  height: 100%;
}
</style>