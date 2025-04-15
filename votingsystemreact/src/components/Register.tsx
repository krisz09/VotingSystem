import React, { useState } from "react";
import { useAuth } from "../context/AuthContext";
import { register as registerApi } from "../api/api"; // ⬅️ átnevezzük az ütközés elkerülésére

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
            const token = await registerApi(email, password); // ⬅️ az új api.ts-beli függvényt hívjuk
            login(token);
            setSuccess(true);
        } catch (err: any) {
            if (err.response?.status === 400) {
                setError("A felhasználó már létezik, vagy a jelszó túl gyenge.");
            } else {
                setError("Hiba történt regisztráció közben.");
            }
        }
    };

    return (
        <div style={{ marginTop: "20px" }}>
            <h2>Regisztráció</h2>
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
                    <label>Jelszó:</label><br />
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                </div>
                <button type="submit">Regisztráció</button>
                {success && (
                    <p style={{ color: "green" }}>Sikeres regisztráció! Be vagy jelentkezve.</p>
                )}
                {error && <p style={{ color: "red" }}>{error}</p>}
            </form>
        </div>
    );
};

export default RegisterForm;