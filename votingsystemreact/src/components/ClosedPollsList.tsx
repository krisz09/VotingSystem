import React, { useState, useEffect } from 'react';
import { useAuth } from "../context/AuthContext";
import { PollResponseDto, getClosedPolls } from "../api/api";

const ClosedPollsList: React.FC = () => {
    const [closedPolls, setClosedPolls] = useState<PollResponseDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const [questionInput, setQuestionInput] = useState('');
    const [startDateInput, setStartDateInput] = useState<string | null>(null);
    const [endDateInput, setEndDateInput] = useState<string | null>(null);

    // Filters used for API requests
    const [questionText, setQuestionText] = useState('');
    const [startDate, setStartDate] = useState<string | null>(null);
    const [endDate, setEndDate] = useState<string | null>(null);

    const { token } = useAuth();

    useEffect(() => {
        if (!token) return;

        setLoading(true);
        setError(null);

        getClosedPolls(token, questionText, startDate, endDate)
            .then(data => {
                setClosedPolls(data);
            })
            .catch(err => {
                console.error("Error fetching closed polls:", err);
                setError("Failed to load closed polls.");
            })
            .finally(() => setLoading(false));
    }, [token, questionText, startDate, endDate]);

    const handleApplyFilters = () => {
        setQuestionText(questionInput);
        setStartDate(startDateInput);
        setEndDate(endDateInput);
    };

    return (
        <div className="closed-polls-container">
            <h2>Closed Polls</h2>

            {/* Filters */}
            <div className="filters">
                <div>
                    <label>Question Text:</label>
                    <input
                        type="text"
                        value={questionInput}
                        onChange={(e) => setQuestionInput(e.target.value)}
                        placeholder="Filter by question text"
                    />
                </div>

                <div>
                    <label>Start Date:</label>
                    <input
                        type="date"
                        value={startDateInput || ''}
                        onChange={(e) => setStartDateInput(e.target.value)}
                    />
                </div>

                <div>
                    <label>End Date:</label>
                    <input
                        type="date"
                        value={endDateInput || ''}
                        onChange={(e) => setEndDateInput(e.target.value)}
                    />
                </div>

                <button onClick={handleApplyFilters}>Apply Filters</button>
            </div>

            {/* Poll List */}
            {loading ? (
                <p>Loading closed polls...</p>
            ) : error ? (
                <p style={{ color: "red" }}>{error}</p>
            ) : closedPolls.length === 0 ? (
                <p>No closed polls available.</p>
            ) : (
                <div className="polls-list">
                    {closedPolls.map(poll => (
                        <div key={poll.id} className="poll-card">
                            <h3>{poll.question}</h3>
                            <p>End Date: {new Date(poll.endDate).toLocaleString()}</p>
                            <button onClick={() => console.log('View Poll:', poll.id)}>View Poll</button>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
};

export default ClosedPollsList;
