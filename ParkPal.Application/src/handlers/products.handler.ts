import {
    IAPProduct,
    InAppPurchase2
} from "@awesome-cordova-plugins/in-app-purchase-2";

import appStore from "@/store";

function setDebugLogLevel(enabled = true) {
    InAppPurchase2.verbosity = InAppPurchase2.DEBUG;
}

function registerProducts() {
    InAppPurchase2.register({
        id: 'no_ads',
        type: InAppPurchase2.CONSUMABLE,
    });

    InAppPurchase2.when("no_ads")
        .approved((p: IAPProduct) => p.verify())
        .verified((p: IAPProduct) => p.finish());

    InAppPurchase2.refresh();
}

function purchaseProduct(product: string): PromiseLike<boolean> {
    return new Promise((resolve, reject) => {
        InAppPurchase2.order(product).then(() => {
                resolve(true);
                appStore.dispatch('setNoAds', true).then();
            },
            () => {
                reject(false);
            }
        );
    });
}

function restorePurchases() {
    InAppPurchase2.refresh();
}

export default {
    setDebugLogLevel,
    registerProducts,
    purchaseProduct,
    restorePurchases
}