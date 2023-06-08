import {AdMob, BannerAdPluginEvents, BannerAdPosition, BannerAdSize, RewardAdOptions} from "@capacitor-community/admob";
import store from '@/store';
import {Capacitor} from "@capacitor/core";
import {Keyboard} from "@capacitor/keyboard";

export function initialiseAdvertisements() {
    if(store.state.isApp) {
        AdMob.initialize({
            requestTrackingAuthorization: true,
            initializeForTesting: false
        }).then(() => {

            store.dispatch('setAdvertisementsInitialised', true).then(() => {
                console.log('Ads Initialised');

                setTimeout(() => {
                    showBannerAdvertisement(store.state.settings.noAds);
                }, 400);


                AdMob.addListener(BannerAdPluginEvents.SizeChanged, (bannerSize) => {
                    // Whenever the banner size changes, we need to set our ad banner height in the store.
                    store.dispatch('setAdHeight', bannerSize.height).then();
                })


                // If we are on android, we need to hide and show advertisements when the keyboard is opened and closed.
                if(Capacitor.getPlatform() == "android") {
                    Keyboard.addListener('keyboardDidShow', () => {
                        store.dispatch('setKeyboardVisible', true).then(() => {
                            hideBannerAdvertisement();
                        });
                    })

                    Keyboard.addListener('keyboardDidHide', () => {
                        store.dispatch('setKeyboardVisible', false).then(() => {
                            resumeBannerAdvertisement(store.state.settings.noAds);
                        });
                    })
                }
            });

        })
    }
}

export function showBannerAdvertisement(noAds: boolean) {
    if(store.state.isApp) {
        if (!noAds) {
            setTimeout(() => {
                const tabs: Element = document.getElementsByTagName('ion-tab-button')[0];
                const margin = tabs.clientHeight + 2;
                let adId = 'ca-app-pub-1263240325581067/3458363938';

                // If this is Android, we need to overwrite the banner Ad ID with the Android one.
                if(Capacitor.getPlatform() == "android") {
                    adId = 'ca-app-pub-1263240325581067/1716514538';
                }

                // Place banner ads above tab bar.
                const bannerOptions = {
                    adId,
                    adSize: BannerAdSize.ADAPTIVE_BANNER,
                    position: BannerAdPosition.BOTTOM_CENTER,
                    margin
                }

                AdMob.showBanner(bannerOptions).then();
            }, 200)
        }
    }
}

export function hideBannerAdvertisement() {
    if(store.state.isApp) {
        AdMob.hideBanner().then();
    }
}

export function resumeBannerAdvertisement(noAds: boolean) {
    if(store.state.isApp) {
        if(!noAds) {
            AdMob.resumeBanner().then();
        }
    }
}

export function showRewardAdvertisement() {
    return new Promise(function(resolve) {
        if(store.state.isApp) {
            let adId = 'ca-app-pub-1263240325581067/4493109970';

            // If this is Android, we need to overwrite the banner Ad ID with the Android one.
            if(Capacitor.getPlatform() == "android") {
                adId = 'ca-app-pub-1263240325581067/4737429544';
            }

            const options: RewardAdOptions = {
                adId
            };

            AdMob.prepareRewardVideoAd(options).then(() => {
                AdMob.showRewardVideoAd().then(rewardItem => {
                    resolve(rewardItem);
                })
            });
        }
    })

}