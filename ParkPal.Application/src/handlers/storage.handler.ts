import Settings from "@/models/store/Settings";
import Destination from "@/models/api/Destination";
import Park from "@/models/api/Park";
import Theme from "@/models/store/theme/Theme";

function getSettingsFromLocalStorage(): Settings {
    const settingsAsJson = localStorage.getItem(`${process.env.VUE_APP_ENVIRONMENT}-settings`) ?? 'null';
    const settings: Settings = new Settings(JSON.parse(settingsAsJson));
    const originalTheme: Theme = settings.theme;

    // Check if the theme has been customised, if it has then we do NOT want to overwrite the theme, otherwise we do.
    if(!settings.theme.custom) {
        settings.theme = new Theme();
        // Then we need to check if this user has had the dark theme enabled before and if they have, then enable it.
        if(originalTheme.darkMode) {
            settings.theme.setDarkTheme();
        }
    }
    return settings;
}

function storeSettingsInLocalStorage(settings: Settings) {
    const settingsAsJsonString = JSON.stringify(settings);
    localStorage.setItem(`${process.env.VUE_APP_ENVIRONMENT}-settings`, settingsAsJsonString);
}

function getActiveDestinationFromLocalStorage(): Destination {
    const destinationAsJson = localStorage.getItem(`${process.env.VUE_APP_ENVIRONMENT}-activeDestination`) ?? 'null';
    const destination: Destination = new Destination(JSON.parse(destinationAsJson));
    return destination;
}

function storeActiveDestinationInLocalStorage(destination: Destination) {
    const destinationAsJsonString = JSON.stringify(destination);
    localStorage.setItem(`${process.env.VUE_APP_ENVIRONMENT}-activeDestination`, destinationAsJsonString);
}

function getActiveParkFromLocalStorage(): Park {
    const parkAsJson = localStorage.getItem(`${process.env.VUE_APP_ENVIRONMENT}-activePark`) ?? 'null';
    const park: Park = new Park(JSON.parse(parkAsJson));
    return park;
}

function storeActiveParkInLocalStorage(park: Park) {
    const parkAsJsonString = JSON.stringify(park);
    localStorage.setItem(`${process.env.VUE_APP_ENVIRONMENT}-activePark`, parkAsJsonString);
}

export const storageHandler = {
    getSettingsFromLocalStorage,
    storeSettingsInLocalStorage,
    getActiveDestinationFromLocalStorage,
    storeActiveDestinationInLocalStorage,
    getActiveParkFromLocalStorage,
    storeActiveParkInLocalStorage
};