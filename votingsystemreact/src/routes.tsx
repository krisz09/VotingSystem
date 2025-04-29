import React from 'react';
import { Routes, Route } from 'react-router-dom';
import Home from './components/Home';
import Login from './components/Login';
import Register from './components/Register';
import ActivePollsPage from "./components/ActivePollsPage";
import ClosedPollsList from './components/ClosedPollsList';
import Navbar from './components/Navbar';

const AppRoutes: React.FC = () => (
    <>
        <Navbar />
        <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />


            <Route path="/active-polls" element={<ActivePollsPage />} />

            <Route path="/closed-polls" element={<ClosedPollsList />} />
        </Routes>
    </>
);

export default AppRoutes;
