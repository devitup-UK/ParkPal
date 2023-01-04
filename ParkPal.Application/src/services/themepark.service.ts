import axios from "axios";
import {authHeader} from "@/helpers/authHeaders.helper";
import AttractionsRequest from "@/models/api/requests/themepark/AttractionsRequest";

const instance = axios.create({
    baseURL: 'https://api-dev.parkpal.co.uk/themepark/',
    timeout: 10000
});

function getDestinations() {
    return instance.get(`destinations`, {
        headers: authHeader()
    });
}

function getParks(destinationId: string) {
    return instance.get(`destinations/${destinationId}/parks`, {
        headers: authHeader()
    });
}

function getAttractions(parkId: string, request: AttractionsRequest) {
    return instance.post(`parks/${parkId}/attractions`, request, {
        headers: authHeader()
    });
}

export const themeparkService = {
    getDestinations,
    getParks,
    getAttractions
};