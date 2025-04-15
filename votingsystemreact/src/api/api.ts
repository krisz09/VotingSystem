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



export async function getActivePolls(token: string): Promise<PollResponseDto[]> {
  const response = await axios.get("https://localhost:7294/api/votes", {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
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
