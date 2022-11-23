import Settings from "@/models/store/Settings";
import Destination from "@/models/api/Destination";
import Park from "@/models/api/Park";

function getSettingsFromLocalStorage(): Settings {
    const settingsAsJson = localStorage.getItem('settings') ?? 'null';
    const settings: Settings = new Settings(JSON.parse(settingsAsJson));
    return settings;
}

function storeSettingsInLocalStorage(settings: Settings) {
    const settingsAsJsonString = JSON.stringify(settings);
    localStorage.setItem('settings', settingsAsJsonString);
}

function getActiveDestinationFromLocalStorage(): Destination {
    const destinationAsJson = localStorage.getItem('activeDestination') ?? 'null';
    const destination: Destination = new Destination(JSON.parse(destinationAsJson));
    return destination;
}

function storeActiveDestinationInLocalStorage(destination: Destination) {
    const destinationAsJsonString = JSON.stringify(destination);
    localStorage.setItem('activeDestination', destinationAsJsonString);
}

function getActiveParkFromLocalStorage(): Park {
    const parkAsJson = localStorage.getItem('activePark') ?? 'null';
    const park: Park = new Park(JSON.parse(parkAsJson));
    return park;
}

function storeActiveParkInLocalStorage(park: Park) {
    const parkAsJsonString = JSON.stringify(park);
    localStorage.setItem('activePark', parkAsJsonString);
}

export const storageService = {
    getSettingsFromLocalStorage,
    storeSettingsInLocalStorage,
    getActiveDestinationFromLocalStorage,
    storeActiveDestinationInLocalStorage,
    getActiveParkFromLocalStorage,
    storeActiveParkInLocalStorage
};