import { authFetch } from "../utils/authFetch";

export interface PollOptionDto {
    id: number;
    optionText: string;
}

export interface PollResponseDto {
    id: number;
    question: string;
    startDate: string;
    endDate: string;
    createdByUserId: string;
    pollOptions: PollOptionDto[];
    hasVoted: boolean;
}

export interface PollOptionResultDto {
    id: number;
    optionText: string;
    voteCount: number;
}

export interface PollResultDto {
    id: number;
    question: string;
    options: PollOptionResultDto[];
}

export interface SubmitVoteRequestDto {
    pollOptionId: number;
    userId: string;
}

const API_URL = "https://localhost:7294/api/votes";
const AUTH_URL = "https://localhost:7294/api/auth";

// Function to get active polls
export async function getActivePolls(): Promise<PollResponseDto[]> {
    const response = await authFetch(`${API_URL}`);
    return await response.json();
}


// Function to login
export async function login(email: string, password: string): Promise<string> {
    const response = await fetch("https://localhost:7294/users/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
    });

    if (!response.ok) {
        throw new Error("Login failed");
    }

    const data = await response.json();
    localStorage.setItem("token", data.authToken);
    localStorage.setItem("refreshToken", data.refreshToken);
    localStorage.setItem("userId", data.userId);

    return data.authToken; // ezt adod tovább a contextnek
}

export async function register(email: string, password: string): Promise<string> {
    const response = await fetch("https://localhost:7294/users/register", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
    });

    if (!response.ok) {
        throw new Error("Registration failed");
    }

    const data = await response.json();
    localStorage.setItem("token", data.authToken);
    localStorage.setItem("refreshToken", data.refreshToken);
    localStorage.setItem("userId", data.userId);

    return data.authToken;
}


export async function getPollResults(pollId: number): Promise<PollResultDto> {
    const response = await authFetch(`${API_URL}/${pollId}/results`);
    return await response.json();
}


export async function submitVote(pollOptionId: number, userId: string): Promise<void> {
    const response = await authFetch(`${API_URL}/submit-vote`, {
        method: "POST",
        body: JSON.stringify({ pollOptionId, userId }),
    });

    if (!response.ok) {
        throw new Error("Failed to submit vote.");
    }
}

export const getClosedPolls = async (
    questionText: string,
    startDate: string | null,
    endDate: string | null
): Promise<PollResponseDto[]> => {
    const params: Record<string, string> = {};
    if (questionText) params.questionText = questionText;
    if (startDate) params.startDate = startDate;
    if (endDate) params.endDate = endDate;

    const queryString = new URLSearchParams(params).toString();

    const response = await authFetch(`${API_URL}/closed-polls?${queryString}`);
    return await response.json();
};
