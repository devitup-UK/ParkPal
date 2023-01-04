import axios, {AxiosResponse} from "axios";
import { authHeader } from '@/helpers/authHeaders.helper';
import SaveSubscriptionRequest from "@/models/api/requests/subscription/SaveSubscriptionRequest";
import Subscription from "@/models/api/Subscription";
import VoucherRequest from "@/models/api/requests/subscription/VoucherRequest";
import Voucher from "@/models/api/Voucher";

const instance = axios.create({
    baseURL: 'https://api.parkpal.co.uk/subscription/',
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

function redeemVoucher(request: VoucherRequest): Promise<Voucher> {
    return new Promise((resolve, reject) => {
        instance.post(`voucher/redeem`, request, {
            headers: authHeader()
        }).then((response: AxiosResponse<Voucher>) => {
            const transformedVoucher = new Voucher(response.data);
            resolve(transformedVoucher);
        }).catch((error) => {
            reject(error);
        });

        return undefined;
    })
}

function verifyVoucher(request: VoucherRequest) {
    return new Promise((resolve, reject) => {
        instance.post(`voucher/verify`, request, {
            headers: authHeader()
        }).then(() => {
            resolve(true);
        }).catch((error) => {
            console.log('Voucher Verification Error', error);
            reject(error);
        });

        return undefined;
    })
}

export const subscriptionService = {
    redeemVoucher,
    save,
    verifyVoucher
};