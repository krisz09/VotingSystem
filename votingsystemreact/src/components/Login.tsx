import React, { useState } from "react";
import { useAuth } from "../context/AuthContext";
import { login as loginApi } from "../api/api"; // ⬅️ fontos: átneveztük, hogy ne ütközzön a useAuth login-nel
import { useNavigate } from "react-router-dom";

const LoginForm: React.FC = () => {
    const { login } = useAuth();
    const navigate = useNavigate();

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError("");

        try {
            const token = await loginApi(email, password); // ⬅️ itt hívjuk az api.ts-ben lévőt
            login(token); // ⬅️ itt hívjuk az AuthContext-es login-t
            navigate("/active-polls");
        } catch (err) {
            setError("Hibás email vagy jelszó.");
        }
    };

    return (
        <div style={{ marginTop: "20px" }}>
            <h2>Login</h2>
            <form onSubmit={handleSubmit}>
                <div>
                    <label>Email:</label><br />
                    <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
                </div>
                <div>
                    <label>Password:</label><br />
                    <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} required />
                </div>
                <button type="submit">Login</button>
                {error && <p style={{ color: "red" }}>{error}</p>}
            </form>
        </div>
    );
};

export default LoginForm;