import React from 'react';
import './App.css';
import AppRoutes from './routes'; // Import the routing logic

const App: React.FC = () => {
    return (
        <div className="App">
            <AppRoutes />
        </div>
    );
};

export default App;
