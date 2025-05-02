export async function refreshTokenIfNeeded(): Promise<string> {
    const refreshToken = localStorage.getItem("refreshToken");

    if (!refreshToken) {
        throw new Error("Nincs refresh token");
    }

    const response = await fetch("https://localhost:7294/users/refresh", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(refreshToken),
    });

    if (!response.ok) {
        throw new Error("Token frissítés sikertelen");
    }

    const data = await response.json();

    localStorage.setItem("token", data.authToken);
    localStorage.setItem("refreshToken", data.refreshToken);
    return data.authToken;
}

export async function authFetch(input: RequestInfo, init: RequestInit = {}): Promise<Response> {
    const token = localStorage.getItem("token");

    const authInit: RequestInit = {
        ...init,
        headers: {
            ...(init.headers || {}),
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
        },
    };

    let response = await fetch(input, authInit);

    if (response.status === 401) {
        try {
            const newToken = await refreshTokenIfNeeded();

            const retryInit: RequestInit = {
                ...init,
                headers: {
                    ...(init.headers || {}),
                    Authorization: `Bearer ${newToken}`,
                    "Content-Type": "application/json",
                },
            };

            response = await fetch(input, retryInit);
        } catch (err) {
            console.error("Token frissítés sikertelen:", err);
            throw err;
        }
    }

    return response;
}
