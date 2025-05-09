import React, { useState } from "react";
import { useSearchParams } from "react-router-dom";
import { resetPassword } from "../api/api";

const ResetPassword: React.FC = () => {
    const [searchParams] = useSearchParams();
    const [newPassword, setNewPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [message, setMessage] = useState("");

    const email = searchParams.get("email");
    const token = decodeURIComponent(searchParams.get("token") || "");


    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setMessage("");

        if (!email || !token) {
            setMessage("❌ Invalid or expired reset link.");
            return;
        }

        if (newPassword !== confirmPassword) {
            setMessage("❗ Passwords do not match.");
            return;
        }

        try {
            await resetPassword(email, token, newPassword);
            setMessage("✅ Password reset successful. You can now log in.");
        } catch (err) {
            setMessage("❌ Failed to reset password. The link may be invalid or expired.");
        }
    };

    return (
        <div style={{ marginTop: "40px" }}>
            <h2>Reset Password</h2>
            <form onSubmit={handleSubmit}>
                <label>New Password:</label><br />
                <input
                    type="password"
                    value={newPassword}
                    onChange={(e) => setNewPassword(e.target.value)}
                    required
                />
                <br />
                <label>Confirm Password:</label><br />
                <input
                    type="password"
                    value={confirmPassword}
                    onChange={(e) => setConfirmPassword(e.target.value)}
                    required
                />
                <br /><br />
                <button type="submit">Set New Password</button>
            </form>

            {message && <p style={{ marginTop: "1rem" }}>{message}</p>}
        </div>
    );
};

export default ResetPassword;
