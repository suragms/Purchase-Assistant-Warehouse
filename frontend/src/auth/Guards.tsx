import React from 'react';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAuthStore } from '../stores/authStore';

export const ProtectedRoute: React.FC = () => {
  const { isAuthenticated, user } = useAuthStore();
  const location = useLocation();

  if (!isAuthenticated || !user) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  return <Outlet />;
};

export const PermissionGate: React.FC<{ permission: string; children: React.ReactNode }> = ({ permission, children }) => {
  const user = useAuthStore((s) => s.user);

  if (!user?.currentBusiness) return null;
  if (user.currentBusiness.role === 'Owner' || user.currentBusiness.role === 'SuperAdmin') return <>{children}</>;

  const hasPerm = user.currentBusiness.permissions?.includes(permission);
  return hasPerm ? <>{children}</> : null;
};
