<template>
  <li :class="`banner ${this.class}`">
    <span>
      <slot></slot>
    </span>
  </li>
</template>

<script lang="ts">
import {defineComponent} from "vue";

export default defineComponent({
  name: "AttractionBanner",
  props: {
    class: {
      type: String,
      default: ''
    }
  }
})
</script>

<style lang="scss" scoped>
.banner {
  position: relative;
  font-size: 10px;
  font-weight: 500;
  padding: 5px 10px;
  margin: 0 2px 0;
  border-radius: 16px;
  color: #FFF;

  &.banner--low-wait {
    background-color: #1CA70A;
  }

  &.banner--thrill-ride {
    background-color: #E35313;
  }

  &.banner--tame-ride {
    background-color: #2e7198;
  }
}
</style>