import {Purchases, PurchasesOfferings} from '@awesome-cordova-plugins/purchases';

import store from "@/store";
import {CustomerInfo, PurchasesPackage} from "cordova-plugin-purchases";
import {Capacitor} from "@capacitor/core";

function setDebugLogLevel(enabled = true) {
    Purchases.setDebugLogsEnabled(true);
}

function initialisePurchases() {
    let apiKey = 'appl_JepMvmLMlmTIhyDKESvccQiEIpz';

    if(Capacitor.getPlatform() == "android") {
        apiKey = 'goog_ZWTNBWYpjUIEKPLbIqfAORHuaHy';
    }

    Purchases.configureWith({
        apiKey
    })

    Purchases.onCustomerInfoUpdated().subscribe((customerInfo: CustomerInfo) => {
        store.dispatch('setParkPalPlus', customerInfo.activeSubscriptions.length).then();
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

function getPurchases(): PromiseLike<Array<string>> {
    return new Promise((resolve) => {
        Purchases.getCustomerInfo().then((customerInfo) => {
            resolve(customerInfo.activeSubscriptions);
        });
    });
}

function presentVoucherAlert() {
    Purchases.presentCodeRedemptionSheet();
}

export default {
    setDebugLogLevel,
    initialisePurchases,
    getProducts,
    getPurchases,
    purchaseProduct,
    restorePurchases,
    presentVoucherAlert
}