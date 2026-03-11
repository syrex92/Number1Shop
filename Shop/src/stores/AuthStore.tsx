import { makeAutoObservable, runInAction } from 'mobx';
import keycloak from '../config/keycloakConfig'; 
import shopConfig from "../config/shopConfig.ts";
import type { RegistrationFormData } from '../components/registration/RegistrationFormFields';

// --- интерфейсы User, Role, LoginResponseData ---
export type Role = 'admin' | 'user';
export interface User {
  id: string;
  name: string;
  role: Role;
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
  userId: string;
  email: string;
}

// Интерфейс хранилища 
export interface AuthStore {
  user: User | null;
  isLoading: boolean;
  error: string | null;
  accessToken: string | null;
  refreshToken: string | null;

  // Геттеры
  isAuthenticated: boolean;
  role: string;

  // Методы
  login: (email: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
  registration: (data: RegistrationFormData) => Promise<void>;
  refreshTokens: () => Promise<boolean>;
  initializeAuth: () => void;
  checkAuth: () => Promise<boolean>;
  clearTokens: () => void;
  isTokenExpired: () => boolean;
  getAuthHeaders: () => Record<string, string>;
  
  // Вспомогательные 
  extractUserFromToken?: () => User | null;
  saveUserToStorage?: () => void;
}

export const createAuthStore = (): AuthStore => {
  // Пытаемся восстановить пользователя из localStorage (если сохраняли раньше)
  const savedUserJson = localStorage.getItem('user');
  let initialUser = null;
  try {
    if (savedUserJson)
      initialUser = JSON.parse(savedUserJson);
      } catch (e) 
      { /* ignore */ }

  const store = {
    user: initialUser as User | null,
    isLoading: false,
    error: null as string | null,
    accessToken: localStorage.getItem('accessToken') || keycloak.token || null,
    refreshToken: null, // Keycloak управляет refresh токеном сам

    // Геттер для isAuthenticated обращается к keycloak
get isAuthenticated(): boolean {
  return !!this.accessToken && !this.isTokenExpired();
},

    get role(): string {
      // Роль из токена keycloak
      const realmAccess = keycloak.realmAccess;
      if (realmAccess?.roles?.includes('admin')) return 'admin';
      if (realmAccess?.roles?.includes('user')) return 'user';
      return this.user?.role || 'guest';
    },

    // login вызывает keycloak
  async login(name: string, password: string): Promise<void> {
  this.isLoading = true;
  this.error = null;
  
  try {
    console.log('Attempting login for:', name);
    
    // Прямой запрос к Keycloak API через Nginx
    const response = await fetch(`${shopConfig.keycloakUrl}realms/shop/protocol/openid-connect/token`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
      },
      body: new URLSearchParams({
        client_id: 'shop-ui',
        grant_type: 'password',
        username: name,
        password: password,
      }),
    });

    const data = await response.json();
    console.log('Login response:', data);

    if (!response.ok) {
      throw new Error('Ошибка авторизации');
    }

    // Сохраняем токен в keycloak instance 
    // Но keycloak.token не обновится автоматически, поэтому используем runInAction
    
    runInAction(() => {
      // Сохраняем токен в store
      this.accessToken = data.access_token;
      localStorage.setItem('accessToken', data.access_token);
      
      // Декодируем JWT токен чтобы получить информацию о пользователе
      try {
        const tokenParts = data.access_token.split('.');
        if (tokenParts.length === 3) {
          const tokenData = JSON.parse(atob(tokenParts[1]));
          console.log('Token data:', tokenData);
          
          this.user = {
            id: tokenData.sub || '',
            name: tokenData.name || tokenData.preferred_username || name,
            role: tokenData.realm_access?.roles?.includes('admin') ? 'admin' : 'user',
            email: tokenData.email,
            username: tokenData.preferred_username || name,
          };
          
          // Сохраняем в localStorage
          this.saveUserToStorage();
        }
      } catch (e) {
        console.error('Error parsing token:', e);
      }
      
      this.isLoading = false;
    });

  } catch (error) {
    runInAction(() => {
      this.error = 'Ошибка авторизации';
      this.isLoading = false;
    });
    console.error('Login error:', error);
  }
},

    // logout теперь вызывает keycloak.logout()
    async logout(): Promise<void> {
      this.isLoading = true;
      try {
        // keycloak.logout() перенаправит браузер на страницу выхода из Keycloak
        // и потом вернет обратно на siteUrl
        await keycloak.logout({ redirectUri: shopConfig.siteUrl });
      } catch (error) {
        console.error('Logout error:', error);
      } finally {
        // Очищаем локальное состояние
        runInAction(() => {
          this.user = null;
          this.accessToken = null;
          this.isLoading = false;
        });
        this.clearTokens(); // Очистит localStorage
      }
    },

    // registration через Keycloak 
async registration(data: RegistrationFormData): Promise<void> {
  this.isLoading = true;
  this.error = null;
  try {
    // Редирект на страницу регистрации Keycloak
    await keycloak.register({
      redirectUri: window.location.origin, // Вернуться на главную после регистрации
      locale: 'ru',
      loginHint: data.email
    });
  } catch (error) {
    runInAction(() => {
      this.error = error instanceof Error ? error.message : 'Ошибка регистрации';
      this.isLoading = false;
    });
    console.error('Register error:', error);
  }
},

    // refreshTokens теперь просто вызывает keycloak.updateToken()
    async refreshTokens(): Promise<boolean> {
      if (!keycloak.authenticated) return false;
      try {
        // updateToken(600) означает "обнови токен, если он истекает через 600 секунд или меньше"
        const refreshed = await keycloak.updateToken(600);
        if (refreshed) {
          runInAction(() => {
            this.accessToken = keycloak.token || null;
          });
          console.log('Token refreshed');
        }
        return true;
      } catch (error) {
        console.error('Failed to refresh token:', error);
        this.logout();
        return false;
      }
    },

    // initializeAuth теперь проверяет статус keycloak
    initializeAuth(): void {
      // keycloak.init вызывается один раз при старте приложения.
      // Здесь мы просто синхронизируем состояние с keycloak.
      if (keycloak.authenticated) {
        this.user = this.extractUserFromToken();
        this.accessToken = keycloak.token || null;
        this.saveUserToStorage();
      } else {
        this.clearTokens();
      }
    },

    // checkAuth просто проверяет keycloak
    async checkAuth(): Promise<boolean> {
  // Проверяем, есть ли токен в localStorage
  const token = localStorage.getItem('accessToken');
  if (!token) return false;
  
  // Проверяем, не истек ли токен
  try {
    const tokenData = JSON.parse(atob(token.split('.')[1]));
    const exp = tokenData.exp * 1000; // в миллисекундах
    if (Date.now() >= exp) {
      // Токен истек, пробуем обновить
      return await this.refreshTokens();
    }
    
    // Токен валиден
    this.accessToken = token;
    this.user = JSON.parse(localStorage.getItem('user') || 'null');
    return true;
  } catch {
    return false;
  }
},

    clearTokens(): void {
  this.accessToken = null;
  this.user = null;
  localStorage.removeItem('user');
  localStorage.removeItem('accessToken'); 
  // Не очищаем keycloak, так как это его ответственность
},

    isTokenExpired(): boolean {
  if (!this.accessToken) return true;
  try {
    const payload = JSON.parse(atob(this.accessToken.split('.')[1]));
    const exp = payload.exp * 1000; // в миллисекундах
    return Date.now() >= exp;
  } catch {
    return true;
  }
},

    getAuthHeaders(): Record<string, string> {
      const headers: Record<string, string> = {
        'Content-Type': 'application/json'
      };
      if (keycloak.authenticated && keycloak.token) {
        headers['Authorization'] = `Bearer ${keycloak.token}`;
      }
      return headers;
    },

    // --- ВСПОМОГАТЕЛЬНЫЙ МЕТОД ---
    extractUserFromToken(): User | null {
      if (!keycloak.tokenParsed) return null;
      const parsed = keycloak.tokenParsed;
      // Парсим информацию из токена Keycloak. Названия полей могут отличаться.
      return {
        id: parsed.sub || '', // sub - стандартный ID пользователя в Keycloak
        name: parsed.name || parsed.preferred_username || '',
        role: parsed.realm_access?.roles?.includes('admin') ? 'admin' : 'user',
        email: parsed.email || '',
        username: parsed.preferred_username || parsed.name,
      };
    },

    saveUserToStorage(): void {
      if (this.user) {
        localStorage.setItem('user', JSON.stringify(this.user));
      }
    }

  };

  return makeAutoObservable(store);
};

export default createAuthStore;