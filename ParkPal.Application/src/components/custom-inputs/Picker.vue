<template>
  <IonRow class="select-filter select-filter--wait" @click="openPicker" :style="`background:${settings.theme.selectionBoxBackground} !important; border-color: ${settings.theme.selectionBoxBorder} !important;`">
    <IonCol cols="6" class="select-filter__label">
      <h3 :style="`color: ${settings.theme.selectionBoxText} !important; background: transparent;`">{{ label }}</h3>
    </IonCol>
    <IonCol cols="6" class="select-filter__input">
      <span>{{ internalValue }}</span>
    </IonCol>
  </IonRow>
</template>

<script lang="ts">
import {defineComponent} from "vue";
import {IonCol, IonRow, PickerButton, PickerColumn, pickerController} from "@ionic/vue";
import {mapState} from "vuex";
import {hideBannerAdvertisement, resumeBannerAdvertisement} from "@/handlers/advertisements.handler";

export default defineComponent({
  name: "PickerComponent",
  props: {
    label: {
      type: String,
      default: ''
    },
    value: {
      type: Number,
      default: 35
    },
    columns: {
      type: Array,
      default: () => []
    },
    buttons: {
      type: Array,
      default: () => []
    }
  },
  components: {
    IonRow,
    IonCol
  },
  computed: {
    ...mapState(['settings'])
  },
  data() : { internalValue: number } {
    return {
      internalValue: 0
    }
  },
  beforeMount() {
    this.internalValue = this.value;
  },
  methods: {
    async openPicker() {
      this.$store.dispatch('setModalOpen', true)
      hideBannerAdvertisement();

      let internalButtons: PickerButton[] = [
        {
          text: 'Confirm',
          handler: (value) => {
            this.internalValue = value.waitTime.value;
            this.$emit('update:modelValue', value.waitTime.value);
            this.$store.dispatch('setModalOpen', false)
            resumeBannerAdvertisement(this.settings.parkPalPlus);
          },
        },
      ];

      internalButtons = internalButtons.concat(this.buttons as PickerButton[]);

      const picker = await pickerController.create({
        columns: this.columns as PickerColumn[],
        buttons: internalButtons
      });

      await picker.present();
    }
  }
})
</script>

<style lang="scss" scoped>
.select-filter {
  border-width: 0 0 1px;
  border-style: solid;
  border-color: #D5D5D5;

  &:nth-child(1) {
    border-width: 1px 0;
  }

  &:nth-child(2) {
    margin-bottom: 16px;
  }
}

.select-filter, .select-filter__label > h3 {
  background: #FFF;
  font-size: 14px;
  color: #9D9D9D;

  .select-filter__label {
    display: flex;
    align-items: center;
    padding-left: 16px;

    h3 {
      margin: 0;
      font-weight: 400;
    }
  }
}

ion-select {
  text-align: right;

  &::part(icon) {
    display: none !important;
  }
}


.select-filter--wait {
  padding: 10px 10px 10px 0;

  .select-filter__input {
    text-align: right;
  }
}
</style>