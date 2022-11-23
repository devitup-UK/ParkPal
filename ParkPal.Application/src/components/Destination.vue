<template>
  <div class="destination">
    <div class="destination__background" :style="'background-image:url(/img/' + destination.image + '), url(/img/destination-no-image.svg)'"></div>
    <div class="destination__details">
      <p :style="`color: ${settings.theme.destinations.text} !important;`">Explore our range of destinations by swiping through.</p>
      <h1 :style="`color: ${settings.theme.destinations.title} !important;`">{{ destination.name }}</h1>
      <h2 :style="`color: ${settings.theme.destinations.location} !important;`">
        <FontAwesomeIcon icon="location-dot" size="1x" fixed-width></FontAwesomeIcon>
        <span>{{ destination.location }}</span>
      </h2>
    </div>
    <div class="destination__elements">
      <img src="@/assets/logo.png">
      <IonButton class="destination-button" color="transparent" expand="block" @click="navigateToParksOrAttractions(destination)" :style="`color: ${settings.theme.destinations.buttonText} !important; background: ${settings.theme.destinations.buttonBackground} !important; border-radius: 10px;`">
        Explore {{ destination.name }}
      </IonButton>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.destination {
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  position: relative;
  background-image: linear-gradient(transparent, white 90%);

  .destination__background {
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    z-index: -1;
    background-size: cover;
    background-position: center;
  }

  .destination__elements {
    text-align: left;
    margin: 0 20px 15px;

    img {
      margin-left: 20px;
    }
  }

  .destination__details {
    margin-top: var(--ion-safe-area-top, 0);


    p {
      color: #bbbbbb;
      font-size: 12px;
    }

    h1 {
      font-size: 20px;
      color: #FFF;
    }

    h2 {
      margin-top: 6px;
      color: #FFF;
      font-size: 17px;
      font-weight: 300;

      span {
        margin-left: 4px;
      }
    }
  }
}
</style>

<script lang="ts">
import { defineComponent } from "vue";
import { IonButton } from "@ionic/vue";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import Destination from "@/models/api/Destination";
import {mapState} from "vuex";

export default defineComponent({
  name: "DestinationComponent",
  components: {
    IonButton,
    FontAwesomeIcon
  },
  computed: {
    ...mapState(['settings'])
  },
  props: ['destination'],
  methods: {
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
    }
  }
});
</script>