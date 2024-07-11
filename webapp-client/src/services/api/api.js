import axios from 'axios';

const axiosOptions = {
    baseURL: process.env.VUE_APP_SYLVRE_API_URL,
    withCredentials: true,
    timeout: 5000
};

const guestInstance = axios.create(axiosOptions);
const authInstance = axios.create(axiosOptions);

authInstance.interceptors.response.use(
    response => response,
    async error => {
        var responseCode = error.response.status;
        if (responseCode !== 401) {
            return Promise.reject(error);
        }

        try {
            await guestInstance.post('/auth/refresh', {}, {
                params: { strategy: 'cookie' }
            });
        }
        catch {
            return Promise.reject(error);
        }

        return authInstance(error.response.config);
    }
);

export default {
    guest: guestInstance,
    auth: authInstance
};