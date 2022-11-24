import {Purchases, PurchasesOfferings} from '@awesome-cordova-plugins/purchases';

import store from "@/store";
import {CustomerInfo, PurchasesPackage} from "cordova-plugin-purchases";

function setDebugLogLevel(enabled = true) {
    Purchases.setDebugLogsEnabled(true);
}

function initialisePurchases() {
    Purchases.configureWith({
        apiKey: 'appl_JepMvmLMlmTIhyDKESvccQiEIpz'
    })
}

function getProducts(): PromiseLike<Array<PurchasesPackage>> {
    return new Promise((resolve) => {
        Purchases.getOfferings().then((response) => {
            const products: Array<PurchasesPackage> = [];

            if (response.current?.monthly) {
                products.push(response.current?.monthly);
            }

            if (response.current?.annual) {
                products.push(response.current?.annual);
            }

            resolve(products);
        });
    });
}

function purchaseProduct(product: PurchasesPackage) {
    return new Promise((resolve, reject) => {
        Purchases.purchasePackage(product).then((purchaserInfo) => {
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
        Purchases.restorePurchases().then((response: CustomerInfo) => {
            if(response.activeSubscriptions.length) {
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