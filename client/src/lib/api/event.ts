import { getCookie } from "$lib/util/cookies";
import { apiBase } from "./api";
import { fetcherPost } from "./fetcher";

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

export interface Event {
    displayId: string;
    vanityUrl?: string;
    title: string;
    shortDescription: string;
    description: string;
    type: EventType;
    settings: EventSettings;
    revealDate: string;
    submissionStartDate: string;
    submissionEndDate: string;
    votingEndDate: string;
}

export interface EventCreateInfo {
    vanityUrl?: string;
    title: string;
    shortDescription: string;
    description: string;
    type: EventType;
    settings: EventSettings;
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
