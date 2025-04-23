import React, { useState } from "react";
import ActivePollsList from "./components/ActivePollsList";
import { useAuth } from "./context/AuthContext";
import LoginForm from "./components/Login";
import RegisterForm from "./components/Register";
import PollDetails from "./components/PollDetails";
import { PollResponseDto } from "./api/api";
import Navbar from "./components/Navbar"; // Import Navbar
import './App.css'; // Import the CSS file

const App: React.FC = () => {
    const { isLoggedIn, logout } = useAuth();
    const [showRegister, setShowRegister] = useState(false);
    const [selectedPoll, setSelectedPoll] = useState<PollResponseDto | null>(null);

    return (
        <div className="App">
            <Navbar /> {/* Include the Navbar here */}
            <div className="container">
                <h1>Voting System</h1>

                {!isLoggedIn ? (
                    <>
                        <div className="button-group">
                            <button onClick={() => setShowRegister(false)} className="button">
                                Login
                            </button>
                            <button onClick={() => setShowRegister(true)} className="button">
                                Register
                            </button>
                        </div>
                        {showRegister ? <RegisterForm /> : <LoginForm />}
                    </>
                ) : (
                    <>
                        <button onClick={logout} className="logout-button">
                            Logout
                        </button>
                        {selectedPoll ? (
                            <PollDetails poll={selectedPoll} onBack={() => setSelectedPoll(null)} />
                        ) : (
                            <ActivePollsList onSelectPoll={setSelectedPoll} />
                        )}
                    </>
                )}
            </div>
        </div>
    );
};

export default App;
