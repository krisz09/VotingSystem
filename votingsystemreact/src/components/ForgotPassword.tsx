import React, { useState } from "react";
import { forgotPassword } from "../api/api";

const ForgotPassword: React.FC = () => {
    const [email, setEmail] = useState("");
    const [message, setMessage] = useState("");
    const [error, setError] = useState("");

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError("");
        setMessage("");

        try {
            await forgotPassword(email);
            setMessage("📧 If this email exists, a reset link has been sent.");
        } catch (err) {
            setError("❌ Failed to send reset email. Please try again later.");
        }
    };

    return (
        <div style={{ marginTop: "40px" }}>
            <h2>Forgot Password</h2>
            <form onSubmit={handleSubmit}>
                <label>Email:</label><br />
                <input
                    type="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    required
                />
                <br /><br />
                <button type="submit">Send Reset Link</button>
            </form>

            {message && <p style={{ color: "green", marginTop: "1rem" }}>{message}</p>}
            {error && <p style={{ color: "red", marginTop: "1rem" }}>{error}</p>}
        </div>
    );
};

export default ForgotPassword;
