import React from 'react';
import { Routes, Route } from 'react-router-dom';
import Home from './components/Home';
import Login from './components/Login';
import Register from './components/Register';
import ActivePollsPage from "./components/ActivePollsPage";
import ClosedPollsList from './components/ClosedPollsList';
import Navbar from './components/Navbar';
import ForgotPassword from './components/ForgotPassword';
import ResetPassword from './components/ResetPassword';

const AppRoutes: React.FC = () => (
    <>
        <Navbar />
        <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route path="/active-polls" element={<ActivePollsPage />} />
            <Route path="/closed-polls" element={<ClosedPollsList />} />
            <Route path="/forgot-password" element={<ForgotPassword />} />
            <Route path="/reset-password" element={<ResetPassword />} />

        </Routes>
    </>
);

export default AppRoutes;
