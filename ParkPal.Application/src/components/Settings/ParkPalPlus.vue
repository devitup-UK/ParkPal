<template>
  <IonContent :style="`background:${settings.theme.background} !important;`">
    <div class="parkpal-plus-card">
      <div class="parkpal-plus-card__header" :style="`background: ${settings.theme.header.background} !important; color: ${settings.theme.header.text} !important;`">
        <strong>Low Wait Time Notifications</strong>
      </div>
      <div class="parkpal-plus-card__content parkpal-plus-card__content--video">
        <div class="video-background">
          <img src="@/assets/premium-notifications.gif" alt="Premium Notifications">
          <img src="@/assets/notification-example.svg" class="video-background__notification" alt="Notification Example">
        </div>
      </div>
      <div class="parkpal-plus-card__footer" :style="`background: ${settings.theme.header.background} !important; color: ${settings.theme.header.text} !important;`">
        <p>Enjoy wait time notifications below 20 minutes when you subscribe to ParkPal+.</p>
      </div>
    </div>
    <div class="parkpal-plus-card">
      <div class="parkpal-plus-card__header" :style="`background: ${settings.theme.header.background} !important; color: ${settings.theme.header.text} !important;`">
        <strong>Unlimited Notifications</strong>
      </div>
      <div class="parkpal-plus-card__content">
          <img src="@/assets/unlimited-notifications.svg" class="video-background__notification" alt="Unlimited Notifications">
      </div>
      <div class="parkpal-plus-card__footer" :style="`background: ${settings.theme.header.background} !important; color: ${settings.theme.header.text} !important;`">
        <p>With ParkPal+ you can enjoy an unlimited amount of notifications, without ParkPal+ you can may only have three.</p>
      </div>
    </div>
    <div class="parkpal-plus-card">
      <div class="parkpal-plus-card__header" :style="'background: ' + settings.theme.header.background + ' !important; color: '+ settings.theme.header.text + ' !important;'">
        <strong>No Ads</strong>
      </div>
      <div class="parkpal-plus-card__content">
        <img src="@/assets/no-ads.svg" alt="No Advertisements">
      </div>
      <div class="parkpal-plus-card__footer" :style="'background: ' + settings.theme.header.background + ' !important; color: '+ settings.theme.header.text + ' !important;'">
        <p>With ParkPal+ say goodbye to Advertisements, no more banners to block your screen space.</p>
      </div>
    </div>
    <div class="parkpal-plus-card">
      <div class="parkpal-plus-card__header" :style="'background: ' + settings.theme.header.background + ' !important; color: '+ settings.theme.header.text + ' !important;'">
        <strong>Custom Colour Themes</strong>
      </div>
      <div class="parkpal-plus-card__content">
        <img src="@/assets/custom-theme.svg" alt="Custom Theming">
      </div>
      <div class="parkpal-plus-card__footer" :style="'background: ' + settings.theme.header.background + ' !important; color: '+ settings.theme.header.text + ' !important;'">
        <p>Take personalisation to the next level by being able to set your own custom app theme.</p>
      </div>
    </div>
    <div class="parkpal-plus-purchase" :style="'background: ' + settings.theme.header.background + ' !important;'" v-if="product != null && !settings.parkPalPlus">
      <p class="parkpal-plus-purchase__cost" :style="'color: ' + settings.theme.header.text + ' !important;'" v-if="!loading">{{ product.product.priceString }} per {{ product.packageType === 'MONTHLY' ? 'month' : 'year' }}</p>
      <div class="parkpal-plus-purchase__button" v-if="!loading">
        <IonButton expand="block" @click="purchase" color="transparent" :style="`color: ${settings.theme.actionButtonText} !important; background: ${settings.theme.actionButtonBackground} !important; border-radius: 8px;`">Purchase</IonButton>
      </div>
      <div class="parkpal-plus-purchase__processing" v-else>
        <LoaderComponent mode="small">Processing Purchase...</LoaderComponent>
      </div>
      <div class="parkpal-plus-purchase__options" v-if="!loading" :style="'color: ' + settings.theme.header.text + ' !important;'">
        <span @click="changeProduct">{{ alternativeSubscriptionPeriod }}</span>
        <span @click="restorePurchase">Restore Purchases</span>
      </div>
    </div>
    <div class="parkpal-plus-purchase" :style="'background: ' + settings.theme.header.background + ' !important;'" v-else>
      <p :style="'color: ' + settings.theme.header.text + ' !important;margin-bottom: 5px;'">Thank you for subscribing to ParkPal+, please enjoy all of the above features!</p>
    </div>
  </IonContent>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import {
  IonButton,
  IonContent,
  alertController
} from "@ionic/vue";
import parkpalPlusHandler from "@/handlers/parkpalPlus.handler";
import {mapState} from "vuex";
import {Package} from "@capgo/capacitor-purchases";
import LoaderComponent from "@/components/Loader.vue";
import store from "@/store";
import {PurchasesPackage} from "cordova-plugin-purchases";

export default defineComponent({
  name: "ParkPalPlus",
  components: {
    LoaderComponent,
    IonContent,
    IonButton
  },
  computed: {
    alternativeSubscriptionPeriod() {
      if(this.product) {
        if(this.product.product.identifier == "parkpalplus_monthly") {
          return "Yearly Payment";
        }else{
          return "Monthly Payment";
        }
      }

      return "Yearly Payment";
    },
    ...mapState(['products', 'settings'])
  },
  data() : { product: PurchasesPackage | null, loading: boolean } {
    return {
      product: null,
      loading: false
    }
  },
  beforeMount() {
    // Get the monthly product.
    console.log('ParkPalPlus Products', this.products);
    this.product = this.products.find((a: PurchasesPackage) => a.product.identifier == "parkpalplus_monthly");
    console.log('ParkPalPlus Product', this.product);
  },
  methods: {
    changeProduct() {
      if(this.product) {
        if(this.product.product.identifier == "parkpalplus_monthly") {
          this.product = this.products.find((a: PurchasesPackage) => a.product.identifier == "parkpalplus_yearly");
        }else{
          this.product = this.products.find((a: PurchasesPackage) => a.product.identifier == "parkpalplus_monthly");
        }
      }
    },
    async restorePurchase() {
      this.loading = true;

      parkpalPlusHandler.restorePurchases().then(async (isSubscribed) => {
        if(isSubscribed) {
          await store.dispatch('setParkPalPlus', true);

          const alert = await alertController.create({
            header: 'Subscription Restored',
            message: 'Your ParkPal+ subscription has been restored.',
            buttons: ['OK'],
          });

          await alert.present();

        }else{
          await store.dispatch('setParkPalPlus', false);

          const alert = await alertController.create({
            header: 'No Subscription to Restore',
            message: 'You have no existing ParkPal+ subscription to restore.',
            buttons: ['OK'],
          });

          await alert.present();
        }

        this.loading = false;
      });
    },
    purchase() {
      if(this.product) {
        this.loading = true;
        parkpalPlusHandler.purchaseProduct(this.product).then(() => {
          this.loading = false;
        }).catch(() => {
          this.loading = false;
        });
      }
    }
  }
})
</script>

<style lang="scss" scoped>
.parkpal-plus-card {
  background: #FFF;
  margin: 10px 20px;
  border-radius: 18px;
  overflow: hidden;

  .parkpal-plus-card__header {
    padding: 16px;
  }

  .parkpal-plus-card__footer {
    padding: 8px 15px 12px;
    text-align: left;
    color: #5A5A5A;
    font-size: 15px;

    p {
      margin: 0;
    }
  }

  .parkpal-plus-card__content {
    img {
      width: 100%;
    }

    &.parkpal-plus-card__content--video {
      height: 140px;
      position: relative;

      .video-background {
        position: absolute;
        top:0;
        left:0;
        right:0;
        bottom: 0;
        z-index: 2;
        overflow: hidden;

        video {
          width: 100%;
          height: 100%;
          object-fit: cover;
        }

        img {
          position: absolute;
          top: 50%;
          left: 50%;
          transform: translate(-50%, -50%);
        }

        .video-background__notification {
          width: 90%;
        }
      }
    }
  }
}

.parkpal-plus-purchase {
  border-width: 2px 0;
  border-color: #363636;
  border-style: solid;
  background: #FFF;
  padding: 0 14px 10px;
  margin: 20px 0 80px;

  .parkpal-plus-purchase__options {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin: 8px 10px;

    span {
      font-size: 14px;
      color: #A8A8A8;
    }
  }

  .parkpal-plus-purchase__processing {
    position: relative;
    height: 90px;
    margin-top: 20px;
  }
}
</style>