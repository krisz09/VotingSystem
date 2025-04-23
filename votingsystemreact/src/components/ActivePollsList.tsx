import React, { useEffect, useState } from "react";
import { PollResponseDto, getActivePolls } from "../api/api";
import { useAuth } from "../context/AuthContext";

interface Props {
    onSelectPoll: (poll: PollResponseDto) => void;
}

const ActivePollsList: React.FC<Props> = ({ onSelectPoll }) => {
    const [polls, setPolls] = useState<PollResponseDto[]>([]);
    const [loading, setLoading] = useState(true);
    const { token } = useAuth(); // 🔑 Getting the token here

    useEffect(() => {
        if (!token) return;
        console.log("Token sent to backend:", token);

        getActivePolls(token)
            .then(data => {
                setPolls(data);
                setLoading(false);
            })
            .catch(err => {
                console.error("Error fetching active polls:", err);
                setLoading(false);
            });
    }, [token]);

    if (loading) return <p>Loading...</p>;

    return (
        <div>
            <h2>Active Polls</h2>
            {polls.length === 0 ? (
                <p>No active polls.</p>
            ) : (
                <ul>
                    {polls.map(poll => (
                        <li key={poll.id} style={{ marginBottom: "10px" }}>
                            <button onClick={() => onSelectPoll(poll)}>
                                <strong>{poll.question}</strong>
                            </button>
                            <div>
                                {poll.hasVoted ? (
                                    <span style={{ color: "green" }}>✔ You have voted</span>
                                ) : (
                                    <span style={{ color: "gray" }}>✖ You haven't voted yet</span>
                                )}
                            </div>
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
};

export default ActivePollsList;
