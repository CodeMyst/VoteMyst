/**
 * Small wrapper functions around fetch.
 */

export interface FetcherResponse<T> {
    status: number;
    ok: boolean;
    message?: string;
    data?: T;
}

export interface FetcherRequest {
    body?: BodyInit;
    bearer?: string;
}

export const fetcherGet = async <T>(
    url: string,
    req: FetcherRequest = {}
): Promise<FetcherResponse<T>> => {
    const res = await fetcher(url, "get", req);

    return {
        status: res.status,
        ok: res.ok,
        data: res.ok ? await res.json() : null,
        message: res.ok ? null : (await res.json())["statusMessage"]
    };
};

export const fetcherPost = async <T>(
    url: string,
    req: FetcherRequest = {}
): Promise<FetcherResponse<T>> => {
    const res = await fetcher(url, "post", req);

    return {
        status: res.status,
        ok: res.ok,
        data: res.ok ? await res.json() : null,
        message: res.ok ? null : (await res.json())["statusMessage"]
    };
};

export const fetcher = async (
    url: string,
    method = "get",
    req: FetcherRequest
): Promise<Response> => {
    const opts = {
        method: method,
        body: req.body,
        headers: {}
    };

    if (req.bearer) {
        opts.headers = {
            "Content-Type": "application/json",
            Authorization: `Bearer ${req.bearer}`
        };
    } else {
        opts.headers = {
            "Content-Type": "application/json"
        };
    }

    return await fetch(url, opts);
};
