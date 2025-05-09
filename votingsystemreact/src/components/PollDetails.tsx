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
    const [selectedOptionIds, setSelectedOptionIds] = useState<number[]>([]);
    const [message, setMessage] = useState<string | null>(null);
    const [hasVoted, setHasVoted] = useState<boolean>(poll.hasVoted);
    const [submitting, setSubmitting] = useState<boolean>(false);

    const toggleOption = (optionId: number) => {
        setSelectedOptionIds(prev =>
            prev.includes(optionId)
                ? prev.filter(id => id !== optionId)
                : [...prev, optionId]
        );
    };

    const handleVote = async () => {
        console.log("userId:", userId);

        if (!userId) {
            setMessage("❗ You must be logged in to vote.");
            return;
        }

        const min = poll.minVotes ?? 1;
        const max = poll.maxVotes;

        if (selectedOptionIds.length < min || selectedOptionIds.length > max) {
            setMessage(`❗ Please select between ${min} and ${max} options.`);
            return;
        }

        try {
            setSubmitting(true);
            await submitVote(selectedOptionIds, userId); // Ezt a backendhez is hozzá kell igazítani!
            setMessage("✅ Vote submitted successfully!");
            setHasVoted(true);
        } catch (error) {
            console.error("Error submitting vote:", error);
            setMessage("❌ There was an error submitting your vote.");
        } finally {
            setSubmitting(false);
        }
    };

    return (
        <div className="card">
            <h2>{poll.question}</h2>

            <p>
                Select between <strong>{poll.minVotes}</strong> and <strong>{poll.maxVotes}</strong> options.
            </p>

            <ul style={{ listStyleType: "none", paddingLeft: 0 }}>
                {poll.pollOptions.map(option => (
                    <li key={option.id} style={{ marginBottom: "10px" }}>
                        <label>
                            <input
                                type="checkbox"
                                value={option.id}
                                disabled={hasVoted}
                                checked={selectedOptionIds.includes(option.id)}
                                onChange={() => toggleOption(option.id)}
                            />
                            {" "}{option.optionText}
                        </label>
                    </li>
                ))}
            </ul>

            {!hasVoted && (
                <button
                    onClick={handleVote}
                    disabled={selectedOptionIds.length === 0 || submitting}
                >
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
