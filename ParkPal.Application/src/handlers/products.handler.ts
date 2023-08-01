import appStore from "@/store";
import {hideBannerAdvertisement} from "@/handlers/advertisements.handler";
import {alertController} from "@ionic/vue";
import Platform = CdvPurchase.Platform;
import {Capacitor} from "@capacitor/core";

function setDebugLogLevel(enabled = true) {
    CdvPurchase.store.verbosity = CdvPurchase.LogLevel.DEBUG;
}

function registerProducts() {
    let platform = CdvPurchase.Platform.APPLE_APPSTORE;

    if(Capacitor.getPlatform() === "android") {
        platform = CdvPurchase.Platform.GOOGLE_PLAY;
    }

    CdvPurchase.store.register({
        id: 'remove_advertisements',
        type: CdvPurchase.ProductType.NON_CONSUMABLE,
        platform
    });

    CdvPurchase.store.when()
        .approved((p) => p.verify())
        .verified((p) => p.finish())
        .finished(() => {
            console.log('Store Finished');
        });

    CdvPurchase.store.initialize().then(() => {
        console.log('Store Initialized');
        CdvPurchase.store.ready(() => {
            console.log('Store Ready');
            const advertisementsOwned = CdvPurchase.store.owned({
                id: "remove_advertisements",
                platform
            })

            if(advertisementsOwned) {
                appStore.dispatch('setNoAds', true).then();
            }
        })
    });
}

function purchaseProduct(product: string): PromiseLike<boolean> {
    return new Promise((resolve, reject) => {
        let platform = CdvPurchase.Platform.APPLE_APPSTORE;
        if(Capacitor.getPlatform() === "android") {
            platform = CdvPurchase.Platform.GOOGLE_PLAY;
        }
        const storeProduct = CdvPurchase.store.get(product, platform);
        const productOffer = storeProduct?.getOffer();
        if (productOffer) {
            CdvPurchase.store.order(productOffer).then((errorReturned) => {
                    if(!errorReturned) {
                        resolve(true);
                        appStore.dispatch('setNoAds', true).then();
                        hideBannerAdvertisement();
                    }else{
                        resolve(false);
                    }
                }).catch(() => {
                    console.log('Purchase Cancelled');
                    reject(false);
            });
        }
    });
}

function restorePurchases() {
    CdvPurchase.store.restorePurchases().then(async () => {
        console.log('Purchases Restored');
        const alert = await alertController.create({
            header: 'Purchases Restored',
            message: 'Your purchases have been restored successfully.',
            buttons: [
                {
                    text: 'OK',
                    role: 'confirm',
                },
            ],
        });

        await alert.present();
    }).catch(() => {
        console.log('Purchases Restore Failed');
    });
}

export default {
    setDebugLogLevel,
    registerProducts,
    purchaseProduct,
    restorePurchases
}