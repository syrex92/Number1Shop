import { makeAutoObservable } from 'mobx';
import { API_BASE_URL } from '../config/constants';

export interface User {
  id: string;
  name: string;
  role: 'admin' | 'user';
  email: string;
  username: string;
}

export interface LoginResponseData {
  accessToken: string;
  refreshToken: string;
  tokenType: string;
  expiresIn: number;
  expiresAt: string;
  refreshTokenExpiresIn: number;
  refreshTokenExpiresAt: string;
  username: string;
  role: string;
  userId?: string;
  email?: string;
}

export interface AuthStore {
  user: User | null;
  isLoading: boolean;
  error: string | null;
  accessToken: string | null;
  refreshToken: string | null;
  isAuthenticated: boolean;
  role: string;

  login: (email: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
  refreshTokens: () => Promise<boolean>;
  initializeAuth: () => void;
  checkAuth: () => Promise<boolean>;
  setTokens: (tokens: LoginResponseData) => void;
  clearTokens: () => void;
  isTokenExpired: () => boolean;
  getAuthHeaders: () => Record<string, string>;
}

export const createAuthStore = (): AuthStore => {
  const store = {
    user: null as User | null,
    isLoading: false,
    error: null as string | null,
    accessToken: localStorage.getItem('accessToken'),
    refreshToken: localStorage.getItem('refreshToken'),

    get isAuthenticated(): boolean {
      return !!this.accessToken && !this.isTokenExpired();
    },

    get role(): string {
      return this.user?.role || 'guest';
    },

    async login(email: string, password: string): Promise<void> {
      this.isLoading = true;
      this.error = null;

      try {
        const response = await fetch(`${API_BASE_URL}/auth/login`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({ email, password }),
        });

        const data = await response.json();

        if (!response.ok) {
          throw new Error(data.message || 'Ошибка авторизации');
        }

        if (!data.success) {
          throw new Error(data.message || 'Ошибка авторизации');
        }

        this.setTokens(data.data);

      } catch (error) {
        this.error = error instanceof Error ? error.message : 'Ошибка авторизации';
        console.error('Login error:', error);
        throw error;
      } finally {
        this.isLoading = false;
      }
    },

    async refreshTokens(): Promise<boolean> {
      if (!this.refreshToken || !this.accessToken) {
        this.logout();
        return false;
      }

      try {
        const response = await fetch(`${API_BASE_URL}/auth/refresh`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            accessToken: this.accessToken,
            refreshToken: this.refreshToken
          }),
        });

        const data = await response.json();

        if (!response.ok || !data.success) {
          throw new Error(data.message || 'Failed to refresh token');
        }

        this.setTokens(data.data);
        return true;
      } catch (error) {
        console.error('Token refresh error:', error);
        this.logout();
        return false;
      }
    },

    async logout(): Promise<void> {
      try {
        if (this.accessToken) {
          await fetch(`${API_BASE_URL}/auth/logout`, {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
              'Authorization': `Bearer ${this.accessToken}`
            },
            body: JSON.stringify({
              refreshToken: this.refreshToken,
              revokeAllSessions: false
            })
          });
        }
      } catch (error) {
        console.error('Logout error:', error);
      } finally {
        this.user = null;
        this.error = null;
        this.clearTokens();
      }
    },

    initializeAuth(): void {
      const savedUser = localStorage.getItem('user');
      if (savedUser) {
        try {
          this.user = JSON.parse(savedUser);
        } catch (error) {
          console.error('Error parsing saved user:', error);
          this.clearTokens();
        }
      }

      if (this.accessToken && this.isTokenExpired()) {
        if (this.refreshToken) {
          this.refreshTokens().catch(console.error);
        } else {
          this.clearTokens();
        }
      }
    },

    async checkAuth(): Promise<boolean> {
      if (!this.accessToken) return false;

      if (this.isTokenExpired()) {
        return await this.refreshTokens();
      }

      return true;
    },

    setTokens(tokens: LoginResponseData): void {
      this.accessToken = tokens.accessToken;
      this.refreshToken = tokens.refreshToken;

      this.user = {
        id: tokens.userId || this.generateIdFromToken(tokens.accessToken),
        name: tokens.username,
        role: tokens.role as 'admin' | 'user',
        email: tokens.email || '',
        username: tokens.username
      };

      localStorage.setItem('accessToken', tokens.accessToken);
      localStorage.setItem('refreshToken', tokens.refreshToken);
      localStorage.setItem('tokenExpiry', tokens.expiresAt);
      localStorage.setItem('user', JSON.stringify(this.user));
    },

    clearTokens(): void {
      this.accessToken = null;
      this.refreshToken = null;
      this.user = null;

      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
      localStorage.removeItem('tokenExpiry');
      localStorage.removeItem('user');
    },

    isTokenExpired(): boolean {
      try {
        const expiry = localStorage.getItem('tokenExpiry');
        if (!expiry) return true;

        const expiryDate = new Date(expiry);
        // Добавляем запас в 1 минуту чтобы избежать race condition
        return Date.now() >= (expiryDate.getTime() - 60000);
      } catch {
        return true;
      }
    },

    getAuthHeaders(): Record<string, string> {
      const headers: Record<string, string> = {
        'Content-Type': 'application/json'
      };

      if (this.accessToken) {
        headers['Authorization'] = `Bearer ${this.accessToken}`;
      }

      return headers;
    },

    generateIdFromToken(token: string): string {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return payload.sub || payload.nameid || `user_${Date.now()}`;
      } catch {
        return `user_${Date.now()}`;
      }
    }
  };

  return makeAutoObservable(store);
};

export default createAuthStore;