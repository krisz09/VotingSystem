import React, { createContext, useState, useEffect, useContext, ReactNode } from "react";
import { jwtDecode } from "jwt-decode";

interface AuthContextType {
    isLoggedIn: boolean;
    token: string | null;
    userId: string | null;
    login: (token: string) => void;
    logout: () => void;
}

interface JwtPayload {
    sub: string; // user ID
    email?: string;
    // Add other fields if needed
}

const AuthContext = createContext<AuthContextType>({
    isLoggedIn: false,
    token: null,
    userId: null,
    login: () => { },
    logout: () => { },
});

export const useAuth = () => useContext(AuthContext);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
    const [token, setToken] = useState<string | null>(null);
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [userId, setUserId] = useState<string | null>(null);

    useEffect(() => {
        const savedToken = localStorage.getItem("token");
        if (savedToken) {
            try {
                const decoded = jwtDecode<JwtPayload>(savedToken);
                setToken(savedToken);
                setUserId(decoded.sub);
                setIsLoggedIn(true);
            } catch (error) {
                console.error("Hibás token:", error);
                logout();
            }
        }
    }, []);

    const login = (newToken: string) => {
        try {
            const decoded = jwtDecode<JwtPayload>(newToken);
            localStorage.setItem("token", newToken);
            setToken(newToken);
            setUserId(decoded.sub);
            setIsLoggedIn(true);
        } catch (error) {
            console.error("Hibás token a login során:", error);
        }
    };

    const logout = () => {
        localStorage.removeItem("token");
        setToken(null);
        setUserId(null);
        setIsLoggedIn(false);
    };

    return (
        <AuthContext.Provider value={{ isLoggedIn, token, userId, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
};
