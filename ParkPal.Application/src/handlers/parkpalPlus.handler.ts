// import {IAPProduct, InAppPurchase2} from "@ionic-native/in-app-purchase-2";
// import {Purchases, PurchasesOfferings} from '@awesome-cordova-plugins/purchases';

import {CapacitorPurchases, Package, PurchaserInfo} from '@capgo/capacitor-purchases';
import store from "@/store";

function setDebugLogLevel(enabled = true) {
    CapacitorPurchases.setDebugLogsEnabled({
        enabled
    }).then();
}

function initialisePurchases() {
    return CapacitorPurchases.setup({
        apiKey: 'appl_JepMvmLMlmTIhyDKESvccQiEIpz'
    });
}

function getProducts(): PromiseLike<Array<Package>> {
    return new Promise((resolve) => {
        const products: Array<Package> = [];

        CapacitorPurchases.getOfferings().then((response) => {
            if (response.offerings.current?.monthly) {
                products.push(response.offerings.current?.monthly);
            }

            if (response.offerings.current?.annual) {
                products.push(response.offerings.current?.annual);
            }
        });

        resolve(products);
    });
}

function purchaseProduct(product: Package) {
    return new Promise((resolve, reject) => {
        CapacitorPurchases.purchasePackage({
            identifier: product.identifier,
            offeringIdentifier: product.offeringIdentifier
        }).then((purchaserInfo) => {
                resolve(purchaserInfo);
                store.dispatch('setParkPalPlus', true).then();
            },
            ({error}) => {
                reject(error);
            }
        );
    });
}

function restorePurchases(): PromiseLike<boolean> {
    return new Promise((resolve) => {
        CapacitorPurchases.restoreTransactions().then((response: { purchaserInfo: PurchaserInfo }) => {
            if(response.purchaserInfo.activeSubscriptions.length) {
                resolve(true);
            }else{
                resolve(false);
            }
        })
    });
}

export default {
    setDebugLogLevel,
    initialisePurchases,
    getProducts,
    purchaseProduct,
    restorePurchases
}