<template>
  <li :class="`feature ${this.class}`">
    <FontAwesomeIcon :icon="icon" class="feature__icon" fixed-width></FontAwesomeIcon>
  </li>
</template>

<script lang="ts">
import {defineComponent} from "vue";
import {FontAwesomeIcon} from "@fortawesome/vue-fontawesome";

export default defineComponent({
  name: "FeatureComponent",
  props: {
    class: {
      type: String,
      default: ''
    },
    icon: {
      type: String,
      default: 'bell'
    }
  },
  components: {
    FontAwesomeIcon
  }
})
</script>

<style lang="scss" scoped>
.feature {
  .feature__icon {
    font-size: 20px;
  }
}
</style>