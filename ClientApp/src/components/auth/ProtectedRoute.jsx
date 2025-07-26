import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

export default function ProtectedRoute({ allowedRoles, children }) {
  const { isAuthenticated, user } = useAuth();
  const location = useLocation();

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  // If allowedRoles is provided, check if user has any of the allowed roles
  if (allowedRoles && !user?.roles?.some(role => allowedRoles.includes(role))) {
    return <Navigate to="/dashboard" replace />;
  }

  return children;
}