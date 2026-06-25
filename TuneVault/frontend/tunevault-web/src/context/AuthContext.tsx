import { createContext, useContext, useEffect, useMemo, useState, type ReactNode } from 'react';
import { authApi } from '../api/services';
import type { AuthResponse } from '../api/types';

interface AuthContextValue {
  user: AuthResponse | null;
  isAuthenticated: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, password: string, displayName: string) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthResponse | null>(() => {
    const raw = localStorage.getItem('user');
    return raw ? JSON.parse(raw) : null;
  });

  useEffect(() => {
    if (user) localStorage.setItem('user', JSON.stringify(user));
    else localStorage.removeItem('user');
  }, [user]);

  const value = useMemo<AuthContextValue>(() => ({
    user,
    isAuthenticated: !!user && !!localStorage.getItem('token'),
    login: async (email, password) => {
      const result = await authApi.login(email, password);
      localStorage.setItem('token', result.token);
      setUser(result);
    },
    register: async (email, password, displayName) => {
      const result = await authApi.register(email, password, displayName);
      localStorage.setItem('token', result.token);
      setUser(result);
    },
    logout: () => {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      setUser(null);
    },
  }), [user]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
