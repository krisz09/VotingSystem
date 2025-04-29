import React from "react";
import ReactDOM from "react-dom/client";
import App from "./App";
import { AuthProvider } from "./context/AuthContext";
import { BrowserRouter as Router } from "react-router-dom";  // Import BrowserRouter

ReactDOM.createRoot(document.getElementById("root")!).render(
    <React.StrictMode>
        <AuthProvider>
            <Router> {/* Wrap the App with Router */}
                <App />
            </Router>
        </AuthProvider>
    </React.StrictMode>
);