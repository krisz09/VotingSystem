import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const Navbar: React.FC = () => {
    const { logout, isLoggedIn } = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate('/login'); // Redirect to login after logout
    };

    return (
        <div className="navbar">
            <div className="title">Voting System</div>

            <div className="navbar-buttons">
                {isLoggedIn ? (
                    <>
                        <Link to="/active-polls">
                            <button className="navbar-button">Active Polls</button>
                        </Link>

                        <Link to="/closed-polls">
                            <button className="navbar-button">Closed Polls</button>
                        </Link>

                        <button className="logout-button" onClick={handleLogout}>
                            Logout
                        </button>
                    </>
                ) : (
                    <>
                        <Link to="/login">
                            <button className="navbar-button">Login</button>
                        </Link>

                        <Link to="/register">
                            <button className="navbar-button">Register</button>
                        </Link>
                    </>
                )}
            </div>
        </div>
    );
};

export default Navbar;
