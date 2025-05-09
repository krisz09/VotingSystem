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
    const [userId, setUserId] = useState<string | null>(null);
    const [isInitialized, setIsInitialized] = useState(false); // 👈 új flag

    const isLoggedIn = !!token;

    // On mount: try loading from localStorage
    useEffect(() => {
        const savedToken = localStorage.getItem("token");
        if (savedToken) {
            try {
                const decoded = jwtDecode<any>(savedToken);
                const userId = decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
                setToken(savedToken);
                setUserId(userId);
            } catch (error) {
                console.error("Hibás token a betöltéskor:", error);
                logout();
            }
        }
        setIsInitialized(true);
    }, []);

    const login = (newToken: string) => {
        try {
            const decoded = jwtDecode<any>(newToken);
            const userId = decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
            localStorage.setItem("token", newToken);
            setToken(newToken);
            setUserId(userId);
        } catch (error) {
            console.error("Hibás token a login során:", error);
        }
    };

    const logout = () => {
        localStorage.removeItem("token");
        localStorage.removeItem("refreshToken");
        localStorage.removeItem("userId");
        setToken(null);
        setUserId(null);
    };

    // While loading, avoid flashing unauthenticated state
    if (!isInitialized) {
        return <div>🔄 Betöltés...</div>; // vagy null
    }

    return (
        <AuthContext.Provider value={{ isLoggedIn, token, userId, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
};
