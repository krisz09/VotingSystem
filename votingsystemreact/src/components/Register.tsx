import React, { useState } from "react";
import { useAuth } from "../context/AuthContext";
import { register as registerApi } from "../api/api"; // ⬅️ renaming to avoid conflicts

const RegisterForm: React.FC = () => {
    const { login } = useAuth();

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const [success, setSuccess] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError("");
        setSuccess(false);

        try {
            const token = await registerApi(email, password); // ⬅️ calling the new function in api.ts
            login(token);
            setSuccess(true);
        } catch (err: any) {
            if (err.response?.status === 400) {
                setError("The user already exists, or the password is too weak.");
            } else {
                setError("An error occurred during registration.");
            }
        }
    };

    return (
        <div style={{ marginTop: "20px" }}>
            <h2>Registration</h2>
            <form onSubmit={handleSubmit}>
                <div>
                    <label>Email:</label><br />
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label>Password:</label><br />
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                </div>
                <button type="submit">Register</button>
                {success && (
                    <p style={{ color: "green" }}>Registration successful! You are logged in.</p>
                )}
                {error && <p style={{ color: "red" }}>{error}</p>}
            </form>
        </div>
    );
};

export default RegisterForm;
