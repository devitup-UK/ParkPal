<template>
  <div class="park-background" :style="`background: url(/img/${park.image});`"></div>
</template>

<script lang="ts">
import Park from "@/models/api/Park";
import {defineComponent} from "vue";

export default defineComponent({
  name: "ParkBackground",
  props: {
    park: {
      type: Park,
      required: true,
      default: new Park()
    }
  }
})
</script>

<style lang="scss" scoped>
.park-background {
  position: absolute;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
  background-position: center center !important;
  background-size: cover !important;

  &::before {
    position: absolute;
    top: 0;
    right: 0;
    bottom: 0;
    left: 0;
    content: '';
    background: rgba(0, 0, 0, 0.75);
  }
}
</style>