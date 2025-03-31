import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Home from './components/Home';
//import Login from './components/Login';
//import Register from './components/Register';
//import Polls from './components/Polls';

const AppRoutes = () => (
    <Router>
        <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route path="/polls" element={<Polls />} />
        </Routes>
    </Router>
);

export default AppRoutes;