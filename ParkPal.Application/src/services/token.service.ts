import axios, {AxiosResponse} from "axios";

const instance = axios.create({
    // baseURL: 'https://api.parkpal.co.uk/token/',
    baseURL: `${process.env.VUE_APP_API}/token/`,
    timeout: 10000
});

function verify(token: string) {
    return instance.post(`verify`, {
        token
    });
}

function generate() {
    return instance.post(`generate`).then((response: AxiosResponse<{ token: string }>) => {
        return response.data.token;
    }).catch(() => {
        return undefined;
    });
}

export const tokenService = {
    verify,
    generate
};