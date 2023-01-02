import {AdMob, BannerAdPluginEvents, BannerAdPosition, BannerAdSize, RewardAdOptions} from "@capacitor-community/admob";
import store from '@/store';

export function initialiseAdvertisements() {
    if(store.state.isApp) {
        AdMob.initialize({
            requestTrackingAuthorization: true,
            initializeForTesting: true,
            testingDevices: ['6B7DE437-F8B2-455D-901B-B637520D19EB']
        }).then()

        AdMob.addListener(BannerAdPluginEvents.SizeChanged, (bannerSize) => {
            // Whenever the banner size changes, we need to set our ad banner height in the store.
            store.dispatch('setAdHeight', bannerSize.height).then();
        })
    }
}

export function showBannerAdvertisement(parkPalPlus: boolean) {
    // if(store.state.isApp) {
    //     if (!parkPalPlus) {
    //         setTimeout(() => {
    //             const tabs: Element = document.getElementsByTagName('ion-tab-button')[0];
    //             const margin = tabs.clientHeight + 2;
    //
    //             // Place banner ads above tab bar.
    //             const bannerOptions = {
    //                 adId: 'ca-app-pub-1263240325581067/3458363938',
    //                 adSize: BannerAdSize.ADAPTIVE_BANNER,
    //                 position: BannerAdPosition.BOTTOM_CENTER,
    //                 margin,
    //                 isTesting: true
    //             }
    //
    //             AdMob.showBanner(bannerOptions).then();
    //         }, 200)
    //     }
    // }
}

export function hideBannerAdvertisement() {
    // if(store.state.isApp) {
    //     AdMob.hideBanner().then();
    // }
}

export function resumeBannerAdvertisement(parkPalPlus: boolean) {
    // if(store.state.isApp) {
    //     if(!parkPalPlus) {
    //         AdMob.resumeBanner().then();
    //     }
    // }
}

export function showRewardAdvertisement() {
    return new Promise(function(resolve) {
        // if(store.state.isApp) {
        //     const options: RewardAdOptions = {
        //         adId: 'ca-app-pub-1263240325581067/4493109970',
        //         isTesting: true
        //     };
        //
        //     AdMob.prepareRewardVideoAd(options).then(() => {
        //         AdMob.showRewardVideoAd().then(rewardItem => {
        //             resolve(rewardItem);
        //         })
        //     });
        // }
        resolve(true);
    })

}