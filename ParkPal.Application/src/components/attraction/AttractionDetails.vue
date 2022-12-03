<template>
  <div class="attraction-details" :class="this.class">
    <template v-if="destinationName.length">
      <h1 v-if="destinationName.length" style="margin-right: 90px; white-space: break-spaces;">{{ destinationName }}</h1>
      <h2>{{ attraction.name }}</h2>
    </template>
    <template v-else>
      <h1>{{ attraction.name }}</h1>
    </template>
    <div class="attraction-details__information">
      <p>{{ attraction.waitTime != null && attraction.waitTime >= 0 ? 'Operating' : attractionStatus[attraction.status] }}</p>
      <p v-if="attraction.waitTime != null">{{ getWaitTime(attraction.waitTime) }}</p>
    </div>
  </div>
</template>

<script lang="ts">
import Attraction from "@/models/api/Attraction";
import {defineComponent} from "vue";
import {AttractionStatus} from "@/models/enums/AttractionStatus";

export default defineComponent({
  name: "AttractionDetails",
  props: {
    attraction: {
      type: Attraction,
      required: true
    },
    class: {
      type: String,
      default: ''
    },
    destinationName: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      attractionStatus: AttractionStatus
    }
  },
  methods: {
    getWaitTime(waitTime: number) {
      if(waitTime) {
        let returnMessage = waitTime + ' minute';

        if(waitTime > 1) {
          returnMessage += 's';
        }

        return returnMessage;
      }

      return 'Walk On'
    },
  }
});
</script>

<style lang="scss" scoped>
.attraction-details {
  width: 100%;
  padding: 10px;

  h1 {
    font-size: 20px;
  }

  &.attraction-details--wait-time {
    h1 {
      text-align: left;
      margin: 0;
    }
  }

  .attraction-details__information {
    display: flex;
    justify-content: space-between;

    p {
      margin: 4px 0 0;
    }
  }
}
</style>