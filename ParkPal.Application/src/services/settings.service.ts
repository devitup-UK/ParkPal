import Settings from "@/models/store/Settings";

function getSettingsFromLocalStorage(): Settings {
    const settingsAsJson = localStorage.getItem('settings') ?? 'null';
    const settings: Settings = new Settings(JSON.parse(settingsAsJson));
    return settings;
}

function storeSettingsInLocalStorage(settings: Settings) {
    const settingsAsJsonString = JSON.stringify(settings);
    localStorage.setItem('settings', settingsAsJsonString);
}



export const settingsService = {
    getSettingsFromLocalStorage,
    storeSettingsInLocalStorage
};