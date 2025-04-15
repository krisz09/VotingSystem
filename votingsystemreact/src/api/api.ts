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
    hasVoted: boolean;
    pollOptions: PollOptionDto[];
}

export interface SubmitVoteRequestDto {
    pollOptionId: number;
    userId: string;
}


const API_URL = "https://localhost:7294/api/votes";

export async function getActivePolls(): Promise<PollResponseDto[]> {
    const response = await axios.get(API_URL);
    return response.data;
}

// api.ts

export async function login(email: string, password: string): Promise<string> {
    const response = await axios.post("https://localhost:7294/api/auth/login", {
        email,
        password,
    });
    return response.data.token;
}

export async function register(email: string, password: string): Promise<string> {
    const response = await axios.post("https://localhost:7294/api/auth/register", {
        email,
        password,
    });
    return response.data.token;
}

export async function submitVote(pollOptionId: number, userId: string): Promise<void> {
    await axios.post("https://localhost:7294/api/votes/submit-vote", {
        pollOptionId,
        userId,
    });
}
