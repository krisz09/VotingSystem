import React, { useState } from "react";
import { PollResponseDto } from "../api/api";
import { useAuth } from "../context/AuthContext";
import { submitVote } from "../api/api";

interface Props {
    poll: PollResponseDto;
    onBack: () => void;
}

const PollDetails: React.FC<Props> = ({ poll, onBack }) => {
    const { userId } = useAuth();
    const [selectedOptionId, setSelectedOptionId] = useState<number | null>(null);
    const [message, setMessage] = useState<string | null>(null);

    const handleVote = async () => {
        if (selectedOptionId !== null && userId) {
            try {
                await submitVote(selectedOptionId, userId);
                setMessage("Vote successfully recorded! 🎉");
            } catch (error) {
                console.error("Voting error:", error);
                setMessage("An error occurred while submitting your vote.");
            }
        }
    };

    return (
        <div>
            <h2>{poll.question}</h2>
            <ul>
                {poll.pollOptions.map(option => (
                    <li key={option.id}>
                        <label>
                            <input
                                type="radio"
                                name="pollOption"
                                value={option.id}
                                checked={selectedOptionId === option.id}
                                onChange={() => setSelectedOptionId(option.id)}
                            />
                            {option.optionText}
                        </label>
                    </li>
                ))}
            </ul>
            <button onClick={handleVote} disabled={selectedOptionId === null}>
                Vote
            </button>
            <br />
            <button onClick={onBack}>Back</button>

            {message && <p style={{ marginTop: "10px", color: "green" }}>{message}</p>}
        </div>
    );
};

export default PollDetails;