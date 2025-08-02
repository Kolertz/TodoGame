import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import Home from './pages/Home';
import Profile from './pages/Profile';
import './App.css';

const App: React.FC = () => {
    return (
        <AuthProvider>
            <Router>
                <div className="app">
                    <Navbar />
                    <div className="content">
                        <Routes>
                            <Route path="/" element={<Home />} />
                            <Route path="/profile" element={<Profile />} />
                        </Routes>
                    </div>
                </div>
            </Router>
        </AuthProvider>
    );
};

const Navbar: React.FC = () => {
    const { isAuthenticated, userProfile, login, logout, register } = useAuth();

    return (
        <nav className="navbar">
            <div className="navbar-brand">
                <Link to="/">TodoGame</Link>
            </div>
            <div className="navbar-actions">
                {isAuthenticated ? (
                    <>
                        <span className="user-greeting">
                            Hello, {userProfile?.firstName || userProfile?.username}
                        </span>
                        <Link to="/profile" className="profile-link">
                            Profile
                        </Link>
                        <button onClick={logout} className="logout-button">
                            Logout
                        </button>
                    </>
                ) : (
                    <>
                        <button onClick={login} className="login-button">
                            Login
                        </button>
                        <button onClick={register} className="register-button">
                            Register
                        </button>
                    </>
                )}
            </div>
        </nav>
    );
};

export default App;