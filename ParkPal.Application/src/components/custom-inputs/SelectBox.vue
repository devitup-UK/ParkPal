<template>
  <IonRow class="select-filter"  :style="`background:${settings.theme.selectionBoxBackground} !important; border-color: ${settings.theme.selectionBoxBorder} !important;`">
    <IonCol cols="6" class="select-filter__label">
      <h3 :style="`color: ${settings.theme.selectionBoxText} !important; background: transparent;`">{{ this.label }}</h3>
    </IonCol>
    <IonCol cols="6" class="select-filter__input">
      <IonSelect interface="action-sheet" v-model="internalValue" cancelText="Cancel" @ionChange="$emit('update:modelValue', $event.target.value)" @ionDismiss="$emit('dismiss')">
        <IonSelectOption v-for="option in options" :key="option.label" :value="option.value">{{ option.label }}</IonSelectOption>
      </IonSelect>
    </IonCol>
  </IonRow>
</template>

<script lang="ts">
import {defineComponent} from "vue";
import {mapState} from "vuex";
import {IonCol, IonRow, IonSelect, IonSelectOption} from "@ionic/vue";
import {interval} from "rxjs";

export default defineComponent({
  name: "SelectBox",
  components: {
    IonRow,
    IonCol,
    IonSelect,
    IonSelectOption
  },
  props: {
    label: {
      type: String,
      default: ''
    },
    options: {
      type: Array,
      default: () => []
    },
    value: {
      type: Number,
      default: 1
    }
  },
  data() {
    return {
      internalValue: 0
    }
  },
  beforeMount() {
   this.internalValue = this.value;
  },
  computed: {
    ...mapState(['settings']),
  },
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