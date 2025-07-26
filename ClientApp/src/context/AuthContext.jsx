import { createContext, useContext, useState, useEffect } from 'react';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [token, setToken] = useState(localStorage.getItem('token'));
  const [user, setUser] = useState(null);

  useEffect(() => {
    if (token) {
      // Parse the JWT token to get user info
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        
        // Extract roles from the token
        const rolesClaim = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || [];
        const roles = Array.isArray(rolesClaim) ? rolesClaim : [rolesClaim];

        // Set user info including roles
        setUser({
          email: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || payload.email,
          id: payload.sub,
          roles: roles
        });
      } catch (error) {
        console.error('Error parsing token:', error);
        logout();
      }
    } else {
      setUser(null);
    }
  }, [token]);

  const login = (newToken) => {
    localStorage.setItem('token', newToken);
    setToken(newToken);
  };

  const logout = () => {
    localStorage.removeItem('token');
    setToken(null);
    setUser(null);
  };

  const value = {
    token,
    user,
    login,
    logout,
    isAuthenticated: !!token
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}; 