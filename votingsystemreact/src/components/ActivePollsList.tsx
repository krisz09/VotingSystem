import React, { useEffect, useState } from "react";
import { getActivePolls, PollResponseDto } from "../api/api"    

interface Props {
    onSelectPoll: (poll: PollResponseDto) => void;
}

const ActivePollsList: React.FC<Props> = ({ onSelectPoll }) => {
    const [polls, setPolls] = useState<PollResponseDto[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        getActivePolls()
            .then((data) => {
                setPolls(data);
                setLoading(false);
            })
            .catch((error) => {
                console.error("Error fetching polls:", error);
                setLoading(false);
            });
    }, []);

    if (loading) return <p>Betöltés...</p>;

    return (
        <div>
            <h2>Aktív szavazások</h2>
            {polls.length === 0 ? (
                <p>Nincs aktív szavazás.</p>
            ) : (
                <ul>
                    {polls.map(poll => (
                        <li key={poll.id}>
                            <button onClick={() => onSelectPoll(poll)} style={{ cursor: "pointer" }}>
                                <strong>{poll.question}</strong> <br />
                                ({new Date(poll.startDate).toLocaleDateString()} - {new Date(poll.endDate).toLocaleDateString()})
                            </button>
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
};

export default ActivePollsList;
