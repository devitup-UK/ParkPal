import axios, {AxiosResponse} from "axios";
import { authHeader } from '@/helpers/authHeaders.helper';
import SaveSubscriptionRequest from "@/models/api/requests/subscription/SaveSubscriptionRequest";
import Subscription from "@/models/api/Subscription";

const instance = axios.create({
    baseURL: 'https://api-dev.parkpal.co.uk/subscription/',
    timeout: 10000
});


function save(request: SaveSubscriptionRequest) {
    return new Promise((resolve, reject) => {
        instance.post(`save`, request, {
            headers: authHeader()
        }).then((response: AxiosResponse<Subscription>) => {
            const transformedSubscription = new Subscription(response.data);
            resolve(transformedSubscription);
        }).catch((error) => {
            reject(error);
        });

        return undefined;
    })
}

export const subscriptionService = {
    save
};