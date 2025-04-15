import React, { useState } from "react";
import ActivePollsList from "./components/ActivePollsList";
import { useAuth } from "./context/AuthContext";
import LoginForm from "./components/Login";
import RegisterForm from "./components/Register";
import PollDetails from "./components/PollDetails";
import { PollResponseDto } from "./api/api";

const App: React.FC = () => {
    const { isLoggedIn, logout } = useAuth();
    const [showRegister, setShowRegister] = useState(false);
    const [selectedPoll, setSelectedPoll] = useState<PollResponseDto | null>(null);

    return (
        <div className="App" style={{ padding: "20px" }}>
            <h1>Szavazó Rendszer</h1>

            {!isLoggedIn ? (
                <>
                    <div style={{ marginBottom: "10px" }}>
                        <button onClick={() => setShowRegister(false)} style={{ marginRight: "10px" }}>
                            Bejelentkezés
                        </button>
                        <button onClick={() => setShowRegister(true)}>Regisztráció</button>
                    </div>
                    {showRegister ? <RegisterForm /> : <LoginForm />}
                </>
            ) : (
                <>
                    <button onClick={logout} style={{ marginBottom: "20px" }}>
                        Kijelentkezés
                    </button>
                    {selectedPoll ? (
                        <PollDetails poll={selectedPoll} onBack={() => setSelectedPoll(null)} />
                    ) : (
                        <ActivePollsList onSelectPoll={setSelectedPoll} />
                    )}
                </>
            )}
        </div>
    );
};

export default App;