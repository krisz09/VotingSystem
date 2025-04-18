﻿import React, { useState } from "react";
import { useAuth } from "../context/AuthContext";
import { login as loginApi } from "../api/api"; // ⬅️ fontos: átneveztük, hogy ne ütközzön a useAuth login-nel

const LoginForm: React.FC = () => {
    const { login } = useAuth();

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError("");

        try {
            const token = await loginApi(email, password); // ⬅️ itt hívjuk az api.ts-ben lévőt
            login(token); // ⬅️ itt hívjuk az AuthContext-es login-t
        } catch (err) {
            setError("Hibás email vagy jelszó.");
        }
    };

    return (
        <div style={{ marginTop: "20px" }}>
            <h2>Bejelentkezés</h2>
            <form onSubmit={handleSubmit}>
                <div>
                    <label>Email:</label><br />
                    <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
                </div>
                <div>
                    <label>Jelszó:</label><br />
                    <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} required />
                </div>
                <button type="submit">Bejelentkezés</button>
                {error && <p style={{ color: "red" }}>{error}</p>}
            </form>
        </div>
    );
};

export default LoginForm;