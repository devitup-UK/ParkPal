import { RawAxiosRequestHeaders } from "axios";
import Settings from "@/models/store/Settings";

export function authHeader(): RawAxiosRequestHeaders {
    const headers: RawAxiosRequestHeaders = {};

    // Get the user object, containing the token from local storage.
    const settingsObject = localStorage.getItem(`${process.env}-settings`);

    // If we have the token, set our header.
    if(settingsObject) {
        const settings: Settings = JSON.parse(settingsObject);

        headers['x-token'] = 'Bearer ' + settings.apiToken;
    }

    return headers;

}