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
    multipart?: boolean;
}

export const fetcherGet = async <T>(
    url: string,
    req: FetcherRequest = {}
): Promise<FetcherResponse<T>> => {
    const res = await fetcher(url, "get", req);

    const fres: FetcherResponse<T> = {
        status: res.status,
        ok: res.ok,
        message: res.ok ? null : (await res.json())["statusMessage"]
    };

    try {
        fres.data = await res.json();
    } catch (_) {
        fres.data = undefined;
    }

    return fres;
};

export const fetcherPost = async <T>(
    url: string,
    req: FetcherRequest = {}
): Promise<FetcherResponse<T>> => {
    const res = await fetcher(url, "post", req);

    const fres: FetcherResponse<T> = {
        status: res.status,
        ok: res.ok,
        message: res.ok ? null : (await res.json())["statusMessage"]
    };

    try {
        fres.data = await res.json();
    } catch (_) {
        fres.data = undefined;
    }

    return fres;
};

export const fetcherDelete = async <T>(
    url: string,
    req: FetcherRequest = {}
): Promise<FetcherResponse<T>> => {
    const res = await fetcher(url, "delete", req);

    const fres: FetcherResponse<T> = {
        status: res.status,
        ok: res.ok,
        message: res.ok ? null : (await res.json())["statusMessage"]
    };

    try {
        fres.data = await res.json();
    } catch (_) {
        fres.data = undefined;
    }

    return fres;
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
        if (req.multipart) {
            opts.headers = {
                Authorization: `Bearer ${req.bearer}`
            };
        } else {
            opts.headers = {
                "Content-Type": "application/json",
                Authorization: `Bearer ${req.bearer}`
            };
        }
    } else {
        if (!req.multipart) {
            opts.headers = {
                "Content-Type": "application/json"
            };
        }
    }

    return await fetch(url, opts);
};
