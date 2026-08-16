import axios from "axios";

const api = axios.create({
    baseURL: import.meta.env.VITE_BACKEND_API_BASE_URL,
    headers: {
        "Content-Type": "application/json"
    }
});

api.interceptors.request.use(
    (config) => {
        console.log("INTERCEPTOR CALLED");

        const token = sessionStorage.getItem("access_token");

        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }

        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

export default api;