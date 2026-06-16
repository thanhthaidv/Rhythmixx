import { Navigate } from 'react-router-dom';

interface ProtectedRouteProps {
  isAuthenticated: boolean;
  children: React.ReactNode;
}

/**
 * ProtectedRoute - Ensures only authenticated users can access routes
 * Redirects to /home (which shows login modal) if not authenticated
 * 
 * Usage:
 * <ProtectedRoute isAuthenticated={isAuthenticated}>
 *   <YourComponent />
 * </ProtectedRoute>
 */
export const ProtectedRoute = ({ isAuthenticated, children }: ProtectedRouteProps) => {
  if (!isAuthenticated) {
    return <Navigate to="/home" replace />;
  }

  return <>{children}</>;
};

export default ProtectedRoute;
