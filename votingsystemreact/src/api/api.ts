import axios from "axios";

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

export interface SubmitVoteRequestDto {
    pollOptionId: number;
    userId: string;
}

const API_URL = "https://localhost:7294/api/votes";
const AUTH_URL = "https://localhost:7294/api/auth";

// Function to get active polls
export async function getActivePolls(token: string): Promise<PollResponseDto[]> {
    try {
        const response = await axios.get(`${API_URL}`, {
            headers: {
                Authorization: `Bearer ${token}`,
            },
        });
        return response.data;
    } catch (error) {
        console.error("Error fetching active polls:", error);
        throw new Error("Failed to fetch active polls.");
    }
}

// Function to login
export async function login(email: string, password: string): Promise<string> {
    try {
        const response = await axios.post(`${AUTH_URL}/login`, { email, password });
        return response.data.token;
    } catch (error) {
        console.error("Login failed:", error);
        throw new Error("Failed to log in.");
    }
}

// Function to register
export async function register(email: string, password: string): Promise<string> {
    try {
        const response = await axios.post(`${AUTH_URL}/register`, { email, password });
        return response.data.token;
    } catch (error) {
        console.error("Registration failed:", error);
        throw new Error("Failed to register.");
    }
}

// Function to submit vote
export async function submitVote(pollOptionId: number, userId: string): Promise<void> {
    try {
        await axios.post(`${API_URL}/submit-vote`, {
            pollOptionId,
            userId,
        });
    } catch (error) {
        console.error("Vote submission failed:", error);
        throw new Error("Failed to submit vote.");
    }
}

// Function to get closed polls (using axios for consistency)
export const getClosedPolls = async (
    token: string,
    questionText: string,
    startDate: string | null,
    endDate: string | null
): Promise<PollResponseDto[]> => {
    const params: Record<string, string> = {};
    if (questionText) params.questionText = questionText;
    if (startDate) params.startDate = startDate;
    if (endDate) params.endDate = endDate;

    const queryString = new URLSearchParams(params).toString();

    const response = await fetch(`https://localhost:7294/api/votes/closed-polls?${queryString}`, {
        method: "GET",
        headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
        },
    });

    if (!response.ok) {
        throw new Error("Failed to fetch closed polls");
    }

    return await response.json();
};