import api from "../api/api";

console.log(
    "BACKEND URL:",
    import.meta.env.VITE_BACKEND_API_BASE_URL
); 
export const login = async () => {

    const clientId = import.meta.env.VITE_AUTH_CLIENT_ID;
    const clientSecret = import.meta.env.VITE_AUTH_CLIENT_SECRET;

    const response = await api.post(
        "/auth/login",
        {
            ClientId: clientId,
            ClientSecret: clientSecret
        }
    );

    console.log("Login response:", response.data);

    sessionStorage.setItem(
        "access_token",
        response.data.token
    );

    return response.data;
};