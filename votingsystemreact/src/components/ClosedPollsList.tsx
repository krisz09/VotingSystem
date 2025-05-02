import React, { useState, useEffect } from 'react';
import { useAuth } from "../context/AuthContext";
import { PollResponseDto, PollResultDto, getClosedPolls, getPollResults } from "../api/api";


const ClosedPollsList: React.FC = () => {
    const [closedPolls, setClosedPolls] = useState<PollResponseDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const [questionInput, setQuestionInput] = useState('');
    const [startDateInput, setStartDateInput] = useState<string | null>(null);
    const [endDateInput, setEndDateInput] = useState<string | null>(null);

    const [questionText, setQuestionText] = useState('');
    const [startDate, setStartDate] = useState<string | null>(null);
    const [endDate, setEndDate] = useState<string | null>(null);

    const [selectedPollResult, setSelectedPollResult] = useState<PollResultDto | null>(null);
    const [resultsLoading, setResultsLoading] = useState(false);
    const [resultsError, setResultsError] = useState<string | null>(null);

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

    const handleViewPoll = (pollId: number) => {
        if (!token) return;

        setResultsLoading(true);
        setResultsError(null);

        getPollResults(pollId, token)
            .then(result => {
                setSelectedPollResult(result);
            })
            .catch(err => {
                console.error("Error fetching poll results:", err);
                setResultsError("Failed to load poll results.");
            })
            .finally(() => setResultsLoading(false));
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
                <>
                    <div className="polls-list">
                        {closedPolls.map(poll => (
                            <div key={poll.id} className="poll-card">
                                <h3>{poll.question}</h3>
                                <p>End Date: {new Date(poll.endDate).toLocaleString()}</p>
                                <button onClick={() => handleViewPoll(poll.id)}>View Poll</button>
                            </div>
                        ))}
                    </div>

                    {/* Poll Results Section */}
                    {resultsLoading && <p>Loading poll results...</p>}

                    {resultsError && <p style={{ color: 'red' }}>{resultsError}</p>}

                    {selectedPollResult && (
                        <div className="poll-results">
                            <h2>Poll Results: {selectedPollResult.question}</h2>

                            <ul>
                                {selectedPollResult.options.map(option => {
                                    const totalVotes = selectedPollResult.options.reduce((sum, opt) => sum + opt.voteCount, 0);
                                    const percentage = totalVotes > 0 ? ((option.voteCount / totalVotes) * 100).toFixed(2) : "0.00";

                                    return (
                                        <li key={option.id} style={{ marginBottom: '10px' }}>
                                            <div><strong>{option.optionText}</strong>: {option.voteCount} votes ({percentage}%)</div>
                                            <div style={{
                                                backgroundColor: '#e0e0e0',
                                                borderRadius: '8px',
                                                overflow: 'hidden',
                                                height: '16px',
                                                marginTop: '4px'
                                            }}>
                                                <div style={{
                                                    width: `${percentage}%`,
                                                    backgroundColor: '#4caf50',
                                                    height: '100%'
                                                }}></div>
                                            </div>
                                        </li>
                                    );
                                })}
                            </ul>

                            <button onClick={() => setSelectedPollResult(null)}>Close Results</button>
                        </div>
                    )}
                </>
            )}
        </div>
    );
};

export default ClosedPollsList;
    