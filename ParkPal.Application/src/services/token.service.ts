import axios, {AxiosResponse} from "axios";

const instance = axios.create({
    baseURL: 'http://192.168.1.96:5002/token/',
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