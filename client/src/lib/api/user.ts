import { apiBase } from "./api";
import { fetcherGet } from "./fetcher";

export enum UserRole {
    USER,
    BANNED,
    MODERATOR,
    ADMIN
}

export interface User {
    displayId: string;
    username: string;
    joinDate: Date;
    role: UserRole;
    avatarUrl: string;
}

export const getUser = async (username: string): Promise<User | undefined> => {
    const res = await fetcherGet<User>(`${apiBase}/user/${username}`);

    if (!res.ok) return undefined;

    return res.data;
};
