import React, { useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';

const Profile: React.FC = () => {
    const { isAuthenticated, userProfile } = useAuth();
    const navigate = useNavigate();

    useEffect(() => {
        if (!isAuthenticated) {
            navigate('/');
        }
    }, [isAuthenticated, navigate]);

    if (!isAuthenticated) {
        return null; // Пока редирект — не рендерим ничего
    }

    return (
        <div className="profile">
            <h1>User Profile</h1>
            {userProfile && (
                <div className="profile-info">
                    <p><strong>Username:</strong> {userProfile.username}</p>
                    {userProfile.firstName && <p><strong>First Name:</strong> {userProfile.firstName}</p>}
                    {userProfile.lastName && <p><strong>Last Name:</strong> {userProfile.lastName}</p>}
                    {userProfile.email && <p><strong>Email:</strong> {userProfile.email}</p>}
                </div>
            )}
        </div>
    );
};

export default Profile;
