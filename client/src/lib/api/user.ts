import { apiBase } from "./api";
import { fetcherGet } from "./fetcher";

export enum UserRole {
    USER,
    BANNED,
    MODERATOR,
    ADMIN
}

export interface User {
    _id: string;
    username: string;
    joinDate: Date;
    role: UserRole;
    avatarUrl: string;
}

export const getUserByUsername = async (username: string): Promise<User | undefined> => {
    const res = await fetcherGet<User>(`${apiBase}/user/username/${username}`);

    if (!res.ok) return undefined;

    return res.data;
};

export const getUserById = async (id: string): Promise<User | undefined> => {
    const res = await fetcherGet<User>(`${apiBase}/user/id/${id}`);

    if (!res.ok) return undefined;

    return res.data;
};
