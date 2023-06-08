import Theme from "@/models/store/theme/Theme";

export default class Settings {
    apiToken?: string;
    hiddenDestinations: Array<string> = [];
    noAds = false;
    favourites: Array<string> = [];
    theme: Theme = new Theme();
    requestedNotifications = false;
    voucher?: string;

    constructor(data: Pick<Settings, "apiToken" | "hiddenDestinations" | "noAds" | "favourites" | "theme" | "requestedNotifications" | "voucher"> | null = null) {
        if(data != null) {
            this.apiToken = data.apiToken;
            this.hiddenDestinations = data.hiddenDestinations;
            this.noAds = data.noAds;
            this.favourites = data.favourites;
            this.theme = new Theme(data.theme);
            this.requestedNotifications = data.requestedNotifications;
            this.voucher = data.voucher;
        }
    }
}