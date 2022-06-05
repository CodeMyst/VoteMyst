import { getCookie } from "$lib/util/cookies";
import { apiBase } from "./api";
import { fetcherGet, fetcherPost } from "./fetcher";
import type { User } from "./user";

interface RegistrationResponse {
    token: string;
}

export const createAccount = async (username: string): Promise<string | undefined | null> => {
    const data = {
        username: username
    };

    const token = getCookie("votemyst-registration");

    const res = await fetcherPost<RegistrationResponse>(`${apiBase}/auth/register`, {
        body: JSON.stringify(data),
        bearer: token
    });

    if (res.ok) return res.data?.token;

    return null;
};

export const getSelf = async (): Promise<User | null> => {
    const token = getCookie("votemyst");

    if (!token) return null;

    const res = await fetcherGet<User>(`${apiBase}/auth/self`, { bearer: token });

    if (res.ok) return res.data;

    return null;
};
