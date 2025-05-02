import React, { useEffect, useState } from "react";
import { PollResponseDto, getActivePolls } from "../api/api";
import { useAuth } from "../context/AuthContext";

interface Props {
    onSelectPoll: (poll: PollResponseDto) => void;
}

const ActivePollsList: React.FC<Props> = ({ onSelectPoll }) => {
    const [polls, setPolls] = useState<PollResponseDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null); // State for errors
    const { token } = useAuth(); // 🔑 Getting the token here

    useEffect(() => {
        if (!token) return;

        setLoading(true);
        setError(null);

        getActivePolls(token)
            .then(data => {
                setPolls(data);
                setLoading(false);
            })
            .catch(err => {
                console.error("Error fetching active polls:", err);
                setError("Failed to load active polls.");
                setLoading(false);
            });
    }, [token]);

    if (loading) return <p>Loading active polls...</p>;
    if (error) return <p style={{ color: "red" }}>{error}</p>;

    return (
        <div className="active-polls-container">
            <h2>Active Polls</h2>
            {polls.length === 0 ? (
                <p>No active polls available.</p>
            ) : (
                polls.map(poll => (
                    <div key={poll.id} className="active-poll-card">
                        <h3>{poll.question}</h3>
                        <div>
                            {poll.hasVoted ? (
                                <span style={{ color: "green" }}>✔ You have voted</span>
                            ) : (
                                <span style={{ color: "gray" }}>✖ You haven't voted yet</span>
                            )}
                        </div>
                        <button onClick={() => onSelectPoll(poll)}>View Poll</button>
                    </div>
                ))
            )}
        </div>
    );
};

export default ActivePollsList;
