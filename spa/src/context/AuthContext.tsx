import React, { createContext, useContext, useEffect, useRef, useState } from 'react';
import keycloak from '../config/keycloak';

interface UserProfile {
    username?: string;
    firstName?: string;
    lastName?: string;
    email?: string;
}

interface AuthContextType {
    isAuthenticated: boolean;
    userProfile: UserProfile | null;
    login: () => void;
    logout: () => void;
    register: () => void;
}

const AuthContext = createContext<AuthContextType | null>(null);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [userProfile, setUserProfile] = useState<UserProfile | null>(null);
    const initializedRef = useRef(false);

    useEffect(() => {
        if (initializedRef.current || keycloak.authenticated !== undefined) return;

        const initializeKeycloak = async () => {
            try {
                const authenticated = await keycloak.init({
                    onLoad: 'check-sso',
                    silentCheckSsoRedirectUri: window.location.origin + '/silent-check-sso.html',
                    pkceMethod: 'S256'
                });

                initializedRef.current = true;
                updateAuthState(authenticated);

                keycloak.onAuthSuccess = () => updateAuthState(true);
                keycloak.onAuthRefreshSuccess = () => updateAuthState(true);
                keycloak.onAuthError = () => updateAuthState(false);
                keycloak.onAuthRefreshError = () => updateAuthState(false);
                keycloak.onAuthLogout = () => updateAuthState(false);
                keycloak.onTokenExpired = () => {
                    keycloak.updateToken(30).then((refreshed: boolean) => {
                        if (refreshed) console.log('Token refreshed');
                    }).catch(() => {
                        updateAuthState(false);
                    });
                };
            } catch (error) {
                console.error('Keycloak initialization error:', error);
            }
        };

        const updateAuthState = async (authenticated: boolean) => {
            setIsAuthenticated(authenticated);
            if (authenticated) {
                try {
                    const profile = await keycloak.loadUserProfile();
                    setUserProfile(profile);
                } catch (error) {
                    console.error('Failed to load user profile:', error);
                }
            } else {
                setUserProfile(null);
            }
        };

        initializeKeycloak();

        return () => {
            keycloak.onAuthSuccess = undefined;
            keycloak.onAuthRefreshSuccess = undefined;
            keycloak.onAuthError = undefined;
            keycloak.onAuthRefreshError = undefined;
            keycloak.onAuthLogout = undefined;
            keycloak.onTokenExpired = undefined;
        };
    }, []);

    const login = () => keycloak.login();
    const logout = () => keycloak.logout();
    const register = () => keycloak.register();

    const contextValue = {
        isAuthenticated,
        userProfile,
        login,
        logout,
        register
    };

    return (
        <AuthContext.Provider value={contextValue}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
};
