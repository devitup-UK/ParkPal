import {Purchases, PurchasesOfferings} from '@awesome-cordova-plugins/purchases';

import {IAPProduct, InAppPurchase2} from "@awesome-cordova-plugins/in-app-purchase-2";

import store from "@/store";
import {CustomerInfo, PurchasesPackage} from "cordova-plugin-purchases";
import {Capacitor} from "@capacitor/core";

function setDebugLogLevel(enabled = true) {
    InAppPurchase2.verbosity = InAppPurchase2.DEBUG;
}

function registerProducts() {
    InAppPurchase2.register({
        id: 'no_ads',
        type: InAppPurchase2.CONSUMABLE,
    });

    InAppPurchase2.when("no_ads").approved((product: IAPProduct) => {
        product.finish();
    })

    InAppPurchase2.refresh();
}

function purchaseProduct(product: string): PromiseLike<boolean> {
    return new Promise((resolve, reject) => {
        InAppPurchase2.order(product).then(() => {
                resolve(true);
                store.dispatch('setNoAds', true).then();
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

function presentVoucherAlert() {
    Purchases.presentCodeRedemptionSheet();
}

export default {
    setDebugLogLevel,
    registerProducts,
    purchaseProduct,
    restorePurchases,
    presentVoucherAlert
}