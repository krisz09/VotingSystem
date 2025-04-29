import React, { useState } from "react";
import ActivePollsList from "../components/ActivePollsList";
import PollDetails from "../components/PollDetails";
import { PollResponseDto } from "../api/api";

const ActivePollsPage: React.FC = () => {
    const [selectedPoll, setSelectedPoll] = useState<PollResponseDto | null>(null);

    return (
        <div className="container">
            {!selectedPoll ? (
                <ActivePollsList onSelectPoll={setSelectedPoll} />
            ) : (
                <PollDetails poll={selectedPoll} onBack={() => setSelectedPoll(null)} />
            )}
        </div>
    );
};

export default ActivePollsPage;
