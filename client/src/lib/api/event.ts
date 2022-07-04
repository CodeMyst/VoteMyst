import { getCookie } from "$lib/util/cookies";
import { apiBase } from "./api";
import { fetcherGet, fetcherPost, type FetcherResponse } from "./fetcher";
import type { Vote } from "./vote";

export enum EventType {
    ART,
    STORY,
    CODING,
    GAMEJAM
}

export enum EventSettings {
    none = 0,

    randomizeEntries = 1 << 0,
    excludeStaffFromWinning = 1 << 1,
    requireVoteToWin = 1 << 2,

    defaultSettings = randomizeEntries | excludeStaffFromWinning
}

export enum VoteType {
    upvote, // every entry can be upvoted (no downvoting)
    simple, // every entry can be rated from 1-5
    categories // multiple categories, each category can be rated from 1-5
}

export interface Event {
    vanityUrl: string;
    title: string;
    shortDescription: string;
    description: string;
    type: EventType;
    settings: EventSettings;
    voteType: VoteType;
    categories?: string[];
    revealDate: string;
    submissionStartDate: string;
    submissionEndDate: string;
    voteEndDate: string;
    hostIds: string[];
}

export interface EventCreateInfo {
    vanityUrl?: string;
    title: string;
    shortDescription: string;
    description: string;
    type: EventType;
    settings: EventSettings;
    voteType: VoteType;
    categories?: string[];
    revealDate: string;
    submissionStartDate: string;
    submissionEndDate: string;
    voteEndDate: string;
}

export interface EventCreateResponse {
    ok: boolean;

    message?: string;

    event?: Event;
}

export interface Entry {
    _id: string;
    eventId: string;
    authorId: string;
    submitDate: string;
    votes: Vote[];
}

export interface ArtEntry extends Entry {
    filename: string;
}

export const createEvent = async (createInfo: EventCreateInfo): Promise<EventCreateResponse> => {
    const token = getCookie("votemyst");

    const res = await fetcherPost<Event>(`${apiBase}/event/`, {
        body: JSON.stringify(createInfo),
        bearer: token
    });

    if (!res.ok) {
        return {
            ok: false,
            message: res.message
        };
    } else {
        return {
            ok: true,
            event: res.data
        };
    }
};

export const getEventsListing = async (): Promise<Event[]> => {
    const token = getCookie("votemyst");

    const res = await fetcherGet<Event[]>(`${apiBase}/event/listing`, {
        bearer: token
    });

    if (res.ok && res.data) return res.data;

    return [];
};

export const getEvent = async (
    vanityUrl: string,
    token: string | undefined
): Promise<Event | null> => {
    const res = await fetcherGet<Event>(`${apiBase}/event/${vanityUrl}`, {
        bearer: token
    });

    if (res.ok && res.data) return res.data;

    return null;
};

export const hasSubmitted = async (
    vanityUrl: string,
    token: string | undefined
): Promise<boolean> => {
    const res = await fetcherGet<null>(`${apiBase}/event/${vanityUrl}/submitted`, {
        bearer: token
    });

    return res.ok;
};

export const createArtSubmission = async (
    vanityUrl: string,
    formData: FormData
): Promise<FetcherResponse<ArtEntry>> => {
    const token = getCookie("votemyst");

    if (!token) return { status: 400, ok: false, message: "Missing token." };

    const res = await fetcherPost<ArtEntry>(`${apiBase}/event/${vanityUrl}/artSubmit`, {
        bearer: token,
        body: formData,
        multipart: true
    });

    return res;
};

export const getArtSubmissions = async (vanityUrl: string): Promise<ArtEntry[]> => {
    const res = await fetcherGet<ArtEntry[]>(`${apiBase}/event/${vanityUrl}/artSubmissions`);

    if (res.ok && res.data) return res.data;

    return [];
};
