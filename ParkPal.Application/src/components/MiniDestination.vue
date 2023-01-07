<template>
  <li class="mini-destination" :style="`background-image: url('/img/${destination.image}')`">
    <div class="mini-destination__details">
      <h1>{{ destination.name }}</h1>
      <h2>
        <span><FontAwesomeIcon icon="location-dot" size="1x" fixed-width /></span>{{ destination.location }}</h2>
    </div>
  </li>
</template>

<script>
import Destination from "@/models/api/Destination";
import {mapState} from "vuex";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";

export default {
  name: "MiniDestination",
  props: {
    destination: {
      type: Destination,
      default: null
    }
  },
  components: {
    FontAwesomeIcon
  },
  computed: {
    ...mapState(['settings'])
  }
}
</script>

<style lang="scss" scoped>
.mini-destination {
  list-style: none;
  padding: 0;
  margin: 0;
  height: 150px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-direction: column;
  background-position: center;
  background-size: cover;
  color: #FFF;
  position: relative;

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

  .mini-destination__details {
    position: relative;
    z-index: 5;
  }
}
</style>