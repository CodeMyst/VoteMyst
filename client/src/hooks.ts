import { apiBase } from "$lib/api/api";
import { fetcherGet } from "$lib/api/fetcher";
import type { User } from "$lib/api/user";
import { getCookieServer } from "$lib/util/cookies";
import type { RequestEvent } from "@sveltejs/kit";

export interface UserSession {
    user: User;
    token: string;
}

export const getSession = async (
    event: RequestEvent<Record<string, string>>
): Promise<UserSession | Record<string, never>> => {
    const cookieHeader = event.request.headers.get("cookie");

    if (!cookieHeader) return {};

    const token = getCookieServer("votemyst", cookieHeader);

    if (!token) return {};

    const user = await fetcherGet<User>(`${apiBase}/auth/self`, { bearer: token });

    if (!user.ok || !user.data) return {};

    return {
        user: user.data,
        token: token
    };
};
