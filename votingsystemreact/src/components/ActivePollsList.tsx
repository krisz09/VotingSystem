import React, { useEffect, useState } from "react";
import { PollResponseDto, getActivePolls } from "../api/api";
import { useAuth } from "../context/AuthContext";

interface Props {
    onSelectPoll: (poll: PollResponseDto) => void;
}

const ActivePollsList: React.FC<Props> = ({ onSelectPoll }) => {
    const [polls, setPolls] = useState<PollResponseDto[]>([]);
    const [loading, setLoading] = useState(true);
    const { token } = useAuth(); // 🔑 Itt kérjük el a token-t

    useEffect(() => {
        if (!token) return;
        console.log("Token sent to backend:", token);

        getActivePolls(token)
            .then(data => {
                setPolls(data);
                setLoading(false);
            })
            .catch(err => {
                console.error("Hiba az aktív szavazások lekérésekor:", err);
                setLoading(false);
            });
    }, [token]);

    if (loading) return <p>Betöltés...</p>;

    return (
        <div>
            <h2>Aktív szavazások</h2>
            {polls.length === 0 ? (
                <p>Nincs aktív szavazás.</p>
            ) : (
                <ul>
                    {polls.map(poll => (
                        <li key={poll.id} style={{ marginBottom: "10px" }}>
                            <button onClick={() => onSelectPoll(poll)}>
                                <strong>{poll.question}</strong>
                            </button>
                            <div>
                                {poll.hasVoted ? (
                                    <span style={{ color: "green" }}>✔ Már szavaztál</span>
                                ) : (
                                    <span style={{ color: "gray" }}>✖ Még nem szavaztál</span>
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
