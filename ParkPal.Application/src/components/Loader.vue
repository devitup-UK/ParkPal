<template>
  <div class="loader-wrapper" :class="{ 'loader-wrapper--small' : mode == 'small' }">
    <div class="loader">
      <div class="loader__rail" :style="`background: ${settings.theme.loadingIcon} !important;`"></div>
      <span></span>
      <span></span>
      <span></span>
      <div class="loader__loop" :style="`border: 0.2em solid ${settings.theme.loadingIcon} !important;`"></div>
    </div>
    <p class="loader__message" :style="`color: ${settings.theme.text} !important;`">
      <slot></slot>
    </p>
  </div>
</template>

<script>
import { defineComponent } from "vue";
import {mapState} from "vuex";

export default defineComponent({
  name: "LoaderComponent",
  computed: {
    ...mapState(['settings'])
  },
  props: {
    mode: {
      type: String,
      default: 'large'
    },
  }
})
</script>

<style lang="scss" scoped>
.loader-wrapper {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);

  &.loader-wrapper--small {
    .loader {
      width: 5em;
      height: 2.5em;

      span {
        transform-origin: 50% -0.85em;
      }
    }

    .loader__message {
      font-size: 12px;
    }
  }
}

.loader {
  margin: 0 auto;
  width: 10em;
  height: 5em;
  position: relative;
  overflow: hidden;
}

.loader__message {
  text-transform: uppercase;
  font-size: 14px;
}

.loader__rail,
.loader__loop {
  content: '';
  position: absolute;
  bottom: 0;
  right: 0;
}

.loader__rail {
  width: inherit;
  height: 0.2em;
  background-color: hsl(0, 7%, 19%);
}

.loader__loop {
  box-sizing: border-box;
  width: 50%;
  height: inherit;
  border: 0.2em solid hsl(0, 7%, 19%);
  border-radius: 50%;
  left: 25%;
}

.loader span {
  position: absolute;
  width: 5%;
  height: 10%;
  background-color: #948c8c;
  border-radius: 50%;
  bottom: 0.2em;
  left: -5%;
  animation: 2s linear infinite;
  transform-origin: 50% -1.85em;
  animation-name: run, rotating;
}

.loader span:nth-child(2) {animation-delay: 0.075s;}
.loader span:nth-child(3) {animation-delay: 0.15s;}

@keyframes run {
  0% {left: -5%;}
  10%, 60% {left: calc((100% - 5%) / 2);}
  70%, 100% {left: 100%;}
}

@keyframes rotating {
  0%, 10% {transform: rotate(0deg);}
  60%, 100% {transform: rotate(-1turn);}
}

</style>