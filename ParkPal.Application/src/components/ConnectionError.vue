<template>
  <div class="connection-error">
    <div class="connection-error__image">
      <img src="@/assets/connection-issues.svg">
    </div>
    <p :style="'color: ' + settings.theme.text + ' !important;'">There appears to be an issue connecting to the ParkPal servers, we are sorry for the inconvenience, use the button below to try again.</p>
    <div class="filter-button">
      <IonButton expand="full" @click="$emit('retry')" color="transparent" :style="`color: ${settings.theme.actionButtonText} !important; background: ${settings.theme.actionButtonBackground} !important;`">
        RETRY
      </IonButton>
    </div>
  </div>
</template>

<script>
import { IonButton } from "@ionic/vue";
import {mapState} from "vuex";

export default {
  name: "ConnectionError",
  components: {
    IonButton
  },
  computed: {
    ...mapState(['settings'])
  }
}
</script>

<style lang="scss" scoped>
.connection-error {
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