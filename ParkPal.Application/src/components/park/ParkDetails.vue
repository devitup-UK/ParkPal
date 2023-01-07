<template>
  <div class="park-details">
    <h1>{{ destinationName }}</h1>
    <h2>{{ park.name }}</h2>
  </div>
</template>

<script lang="ts">
import Park from "@/models/api/Park";
import {defineComponent} from "vue";
import {mapState} from "vuex";

export default defineComponent({
  name: "ParkDetails",
  props: {
    park: {
      type: Park,
      required: true,
      default: new Park()
    },
    destinationName: {
      type: String,
      required: true,
      default: ''
    }
  },
  computed: {
    ...mapState(['activeDestination'])
  }
})
</script>

<style lang="scss" scoped>
.park-details {
  position: relative;
  z-index: 5;
  margin: 10px 0;
  padding: 0 15px;

  h1 {
    font-size: 20px;
  }

  h2 {
    margin: 4px 0 0;
  }
}
</style>