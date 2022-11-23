<template>
  <IonPage ref="page">
    <IonHeader :style="'background: ' + settings.theme.header.background + ' !important;'">
      <IonToolbar color="transparent" :style="'background: ' + settings.theme.header.background + ' !important;'">
        <IonButtons slot="start">
          <IonButton @click="backToSettings">
            <FontAwesomeIcon icon="arrow-left" :color="settings.theme.header.icons" fixed-width></FontAwesomeIcon>
          </IonButton>
        </IonButtons>
        <IonTitle :style="'background: ' + settings.theme.header.background + ' !important; border-width: 0 0px 2px; border-style: solid;border-color: ' + settings.theme.header.border + ' !important; color: '+ settings.theme.header.text + ' !important;'">Custom Theming</IonTitle>
      </IonToolbar>
    </IonHeader>
    <IonContent :style="`background:${settings.theme.background} !important;`">
      <h3 :style="`color:${settings.theme.text} !important;`" class="ion-text-start ion-padding-start">Presets</h3>
      <p :style="`color:${settings.theme.text} !important;`" class="ion-text-start ion-padding-start ion-padding-end">Set all theme settings to match the chosen preset theme.</p>
      <IonList lines="full" class="ion-margin-top settings-list" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
        <IonItem @click="theme.setDarkTheme()" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Dark Mode</IonLabel>
        </IonItem>
        <IonItem @click="theme.setLightTheme()" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Light Mode</IonLabel>
        </IonItem>
      </IonList>
      <h3 :style="`color:${settings.theme.text} !important;`" class="ion-text-start ion-padding-start">General</h3>
      <IonList lines="full" class="ion-margin-top settings-list" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Background</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.background" class="color-picker">
          </IonLabel>
        </IonItem>
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Text</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.text" class="color-picker">
          </IonLabel>
        </IonItem>
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Loading Icon</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.loadingIcon" class="color-picker">
          </IonLabel>
        </IonItem>
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Action Button</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.actionButtonBackground" class="color-picker">
          </IonLabel>
        </IonItem>
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Action Button Text</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.actionButtonText" class="color-picker">
          </IonLabel>
        </IonItem>
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Reset Button</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.resetButtonBackground" class="color-picker">
          </IonLabel>
        </IonItem>
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Reset Button Text</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.resetButtonText" class="color-picker">
          </IonLabel>
        </IonItem>
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Select Box Background</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.selectionBoxBackground" class="color-picker">
          </IonLabel>
        </IonItem>
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Select Box Border</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.selectionBoxBorder" class="color-picker">
          </IonLabel>
        </IonItem>
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Select Box Text</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.selectionBoxText" class="color-picker">
          </IonLabel>
        </IonItem>
      </IonList>
      <h3 :style="`color:${settings.theme.text} !important;`" class="ion-text-start ion-padding-start">Header</h3>
      <IonList lines="full" class="ion-margin-top settings-list" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Background</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.header.background" class="color-picker">
          </IonLabel>
        </IonItem>
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Border</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.header.border" class="color-picker">
          </IonLabel>
        </IonItem>
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Icons</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.header.icons" class="color-picker">
          </IonLabel>
        </IonItem>
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Text</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.header.text" class="color-picker">
          </IonLabel>
        </IonItem>
      </IonList>
      <h3 :style="`color:${settings.theme.text} !important;`" class="ion-text-start ion-padding-start">Navigation</h3>
      <IonList lines="full" class="ion-margin-top settings-list" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Background</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.navigation.background" class="color-picker">
          </IonLabel>
        </IonItem>
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Border</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.navigation.border" class="color-picker">
          </IonLabel>
        </IonItem>
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Icons</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.navigation.icons" class="color-picker">
          </IonLabel>
        </IonItem>
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Text</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.navigation.text" class="color-picker">
          </IonLabel>
        </IonItem>
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Active Icon</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.navigation.activeIcon" class="color-picker">
          </IonLabel>
        </IonItem>
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Active Text</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.navigation.activeText" class="color-picker">
          </IonLabel>
        </IonItem>
      </IonList>
      <h3 :style="`color:${settings.theme.text} !important;`" class="ion-text-start ion-padding-start">Destinations</h3>
      <IonList lines="full" class="ion-margin-top settings-list" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Text</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.destinations.text" class="color-picker">
          </IonLabel>
        </IonItem>
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Title</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.destinations.title" class="color-picker">
          </IonLabel>
        </IonItem>
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Location</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.destinations.location" class="color-picker">
          </IonLabel>
        </IonItem>
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Button</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.destinations.buttonBackground" class="color-picker">
          </IonLabel>
        </IonItem>
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Button Text</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.destinations.buttonText" class="color-picker">
          </IonLabel>
        </IonItem>
      </IonList>
      <h3 :style="`color:${settings.theme.text} !important;`" class="ion-text-start ion-padding-start">Settings</h3>
      <IonList lines="full" class="ion-margin-top settings-list" :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Background</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.settings.settingBackground" class="color-picker">
          </IonLabel>
        </IonItem>
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Text</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.settings.settingText" class="color-picker">
          </IonLabel>
        </IonItem>
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Arrow</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.settings.settingArrow" class="color-picker">
          </IonLabel>
        </IonItem>
        <IonItem :style="`background:${settings.theme.settings.settingBackground} !important; border-color: ${settings.theme.settings.settingBorder} !important;color: ${settings.theme.settings.settingText} !important;`">
          <IonLabel>Border</IonLabel>
          <IonLabel>
            <input type="color" v-model="theme.settings.settingBorder" class="color-picker">
          </IonLabel>
        </IonItem>
      </IonList>
      <p :style="`color:${settings.theme.text} !important;`" class="ion-text-start ion-padding-start ion-padding-end">Saving your custom theming settings will change the applications design, until you save these settings, the theming will not change.</p>
      <IonRow class="filter-button">
        <IonCol>
          <IonButton expand="full" @click="saveTheme" color="transparent" :style="`color: ${settings.theme.actionButtonText} !important; background: ${settings.theme.actionButtonBackground} !important;`">
            SAVE THEME
          </IonButton>
        </IonCol>
      </IonRow>
    </IonContent>
  </IonPage>
</template>

<style lang="scss" scoped>
.filter-button {
  margin-bottom: 20px;

  ion-col {
    padding: 0;

    ion-button {
      margin: 0;
      font-weight: 300;
      font-size: 14px;
    }
  }
}

ion-label {
  margin-bottom: 6px !important;
}

ion-item::part(native) {
  background: inherit;
  font-size: 14px;
  color: inherit;
  padding-left: 12px;
  border-color: inherit;
}

ion-item::part(detail-icon) {
  color: inherit;
}

.item-native {
  border-color: inherit !important;
}

.settings-list {
  border-top: 1px solid var(--ion-item-border-color, var(--ion-border-color, var(--ion-color-step-250, #c8c7cc)));;
}

.settings-icon {
  font-size: 20px;
  margin-right: 5px;
}

.color-picker {
  float: right;
  height: 30px;
  width: 30px;
  border-radius: 0;
  background: transparent !important;
  border: none;
  outline: none;
  -webkit-appearance: none;
  -moz-appearance: none;
  appearance: none;
  cursor: pointer;
  padding: 0;
}
</style>

<script>
import { defineComponent } from "vue";
import {mapState} from "vuex";
import { IonPage, IonContent, IonHeader, IonToolbar, IonTitle, IonButton, IonButtons, IonList, IonLabel, IonItem, IonRow, IonCol } from "@ionic/vue";
import {FontAwesomeIcon} from "@fortawesome/vue-fontawesome";
import Theme from "@/models/store/theme/Theme";


export default defineComponent({
  name: "CustomThemingView",
  components: {
    IonLabel,
    IonPage,
    IonContent,
    IonHeader,
    IonToolbar,
    IonTitle,
    IonItem,
    IonList,
    IonButtons,
    IonButton,
    IonCol,
    IonRow,
    FontAwesomeIcon
  },
  computed: {
    ...mapState(['settings'])
  },
  data() {
    return {
      theme: new Theme()
    }
  },
  beforeMount() {
    let themeValue = JSON.stringify(this.settings.theme);
    let theme = JSON.parse(themeValue);
    this.theme = new Theme(theme);
  },
  methods: {
    saveTheme() {
      let themeValue = JSON.stringify(this.theme);
      let theme = JSON.parse(themeValue);
      this.$store.dispatch('setTheme', new Theme(theme));
    },
    navigate(route) {
      this.$router.push({
        name: route
      })
    },
    backToSettings() {
      this.$router.push({
        name: 'settings',
        params: {
          transition: 'slide-left'
        }
      })
    }
  }
})
</script>