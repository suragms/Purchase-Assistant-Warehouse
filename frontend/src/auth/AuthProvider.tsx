import React, { useEffect, useRef, useState } from 'react';
import { useAuthStore } from '../stores/authStore';
import apiClient from '../api/apiClient';
import { BrandLoading } from '../components/BrandIdentity';
import { useQueryClient } from '@tanstack/react-query';
import type { User, AuthResponse } from '../types/auth';

const sessionScope = (user: User | null) => JSON.stringify([user?.id, user?.currentBusiness?.businessId,
  user?.currentBusiness?.role, [...(user?.currentBusiness?.permissions || [])].sort()]);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [isLoading, setIsLoading] = useState(true);
  const setSession = useAuthStore((s) => s.setSession);
  const logout = useAuthStore((s) => s.logout);
  const queryClient = useQueryClient();
  const user = useAuthStore(s => s.user);
  const refreshRequest = useRef<Promise<{ data: AuthResponse }> | null>(null);

  // Clear prior data synchronously before a changed identity/tenant/permission scope renders.
  useEffect(() => useAuthStore.subscribe((next, previous) => {
    if (sessionScope(next.user) !== sessionScope(previous.user)) queryClient.clear();
  }), [queryClient]);

  useEffect(() => {
    let active = true;
    const initAuth = async () => {
      try {
        refreshRequest.current ??= apiClient.post<AuthResponse>('/auth/refresh');
        const { data } = await refreshRequest.current;
        if (active && data?.data) {
          setSession(data.data.accessToken, data.data.user);
        }
      } catch (err) {
        if (active) logout();
      } finally {
        if (active) setIsLoading(false);
      }
    };
    initAuth();
    return () => { active = false; };
  }, [setSession, logout]);

  if (isLoading) {
    return (
      <BrandLoading message="Opening your workspace…" fullScreen />
    );
  }

  return <React.Fragment key={sessionScope(user)}>{children}</React.Fragment>;
};
