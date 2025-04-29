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
    const [hasVoted, setHasVoted] = useState<boolean>(poll.hasVoted);
    const [submitting, setSubmitting] = useState<boolean>(false);

    const handleVote = async () => {
        if (selectedOptionId !== null && userId) {
            try {
                setSubmitting(true);
                await submitVote(selectedOptionId, userId);
                setMessage("✅ Vote submitted successfully!");
                setHasVoted(true);
            } catch (error) {
                console.error("Error submitting vote:", error);
                setMessage("❌ There was an error submitting your vote.");
            } finally {
                setSubmitting(false);
            }
        } else {
            setMessage("❗ Please select an option before submitting.");
        }
    };

    return (
        <div className="card">
            <h2>{poll.question}</h2>

            <ul style={{ listStyleType: "none", paddingLeft: 0 }}>
                {poll.pollOptions.map(option => (
                    <li key={option.id} style={{ marginBottom: "10px" }}>
                        <label>
                            <input
                                type="radio"
                                name="pollOption"
                                value={option.id}
                                disabled={hasVoted}
                                checked={selectedOptionId === option.id}
                                onChange={() => setSelectedOptionId(option.id)}
                            />
                            {" "}{option.optionText}
                        </label>
                    </li>
                ))}
            </ul>

            {!hasVoted && (
                <button onClick={handleVote} disabled={selectedOptionId === null || submitting}>
                    {submitting ? "Submitting..." : "Submit Vote"}
                </button>
            )}

            <br />
            <button onClick={onBack} style={{ marginTop: "15px" }}>
                ← Back to Polls
            </button>

            {message && (
                <p style={{ marginTop: "15px", color: hasVoted ? "green" : "red" }}>{message}</p>
            )}
        </div>
    );
};

export default PollDetails;
