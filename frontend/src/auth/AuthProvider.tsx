import React, { useEffect, useState } from 'react';
import { useAuthStore } from '../stores/authStore';
import apiClient from '../api/apiClient';
import { Loader2 } from 'lucide-react';

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [isLoading, setIsLoading] = useState(true);
  const setSession = useAuthStore((s) => s.setSession);
  const logout = useAuthStore((s) => s.logout);

  useEffect(() => {
    const initAuth = async () => {
      try {
        const { data } = await apiClient.post('/auth/refresh');
        if (data?.data) {
          setSession(data.data.accessToken, data.data.user);
        }
      } catch (err) {
        logout();
      } finally {
        setIsLoading(false);
      }
    };
    initAuth();
  }, [setSession, logout]);

  if (isLoading) {
    return (
      <div className="h-screen w-full flex items-center justify-center bg-gray-50">
        <Loader2 className="animate-spin h-10 w-10 text-emerald-600" />
      </div>
    );
  }

  return <>{children}</>;
};