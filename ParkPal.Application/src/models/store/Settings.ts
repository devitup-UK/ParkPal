import Theme from "@/models/store/theme/Theme";

export default class Settings {
    apiToken?: string;
    hiddenDestinations: Array<string> = [];
    parkPalPlus = false;
    favourites: Array<string> = [];
    theme: Theme = new Theme();

    constructor(data: Pick<Settings, "apiToken" | "hiddenDestinations" | "parkPalPlus" | "favourites" | "theme"> | null = null) {
        if(data != null) {
            this.apiToken = data.apiToken;
            this.hiddenDestinations = data.hiddenDestinations;
            this.parkPalPlus = data.parkPalPlus;
            this.favourites = data.favourites;
            this.theme = new Theme(data.theme);
        }
    }
}