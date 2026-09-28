import { create } from 'zustand';
import type { User, BusinessContext } from '../types/auth';

interface AuthState {
  accessToken: string | null;
  user: User | null;
  isAuthenticated: boolean;
  setSession: (token: string, user: User) => void;
  logout: () => void;
  switchBusiness: (context: BusinessContext) => void;
}

export const useAuthStore = create<AuthState>((set) => ({
  accessToken: null,
  user: null,
  isAuthenticated: false,
  setSession: (token, user) => set({ accessToken: token, user, isAuthenticated: !!token }),
  logout: () => set({ accessToken: null, user: null, isAuthenticated: false }),
  switchBusiness: (context) => set((state) => ({
    user: state.user ? { ...state.user, currentBusiness: context } : null
  }))
}));