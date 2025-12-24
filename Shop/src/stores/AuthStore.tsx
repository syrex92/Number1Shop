import { makeAutoObservable } from 'mobx';
import shopConfig from "../config/shopConfig.ts";
import type { RegistrationFormData } from '../components/registration/RegistrationFormFields';

// Интерфейс пользователь
export interface User {
  id: string;           // Guid из C#
  name: string;
  role: 'admin' | 'user';
  email: string;
  username: string;
}

// Интерфейс ответа от сервера 
export interface LoginResponseData {
  accessToken: string;
  refreshToken: string;
  tokenType: string;                // Тип токена ("Bearer")
  expiresIn: number;                // Время жизни access token в секундах
  expiresAt: string;                // Дата и время истечения access token
  refreshTokenExpiresIn: number;    // Время жизни refresh token в секундах
  refreshTokenExpiresAt: string;    // Дата и время истечения refresh token
  username: string;
  role: string;
  userId: string;                  // ID пользователя (Guid)
  email: string;                   // Email пользователя
}


// Интерфейс хранилища аутентификации
export interface AuthStore {
  user: User | null;                // Данные текущего пользователя
  isLoading: boolean;               // Флаг загрузки
  error: string | null;
  accessToken: string | null;
  refreshToken: string | null;

  isAuthenticated: boolean;         // true если пользователь авторизован и токен валиден
  role: string;                     // Роль текущего пользователя или 'guest' если не авторизован

  // ОСНОВНЫЕ МЕТОДЫ АУТЕНТИФИКАЦИИ
  login: (email: string, password: string) => Promise<void>;    // Вход в систему
  logout: () => Promise<void>;                                  // Выход из системы
  registration: (data: RegistrationFormData) => Promise<void>   // Регистрация нового пользователя
  refreshTokens: () => Promise<boolean>;                        // Обновление токенов
  initializeAuth: () => void;                                   // Инициализация при загрузке приложения
  checkAuth: () => Promise<boolean>;                            // Проверка валидности аутентификации

  // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ
  setTokens: (tokens: LoginResponseData) => void;               // Сохранение токенов и данных пользователя
  clearTokens: () => void;                                      // Очистка токенов и данных пользователя
  isTokenExpired: () => boolean;                                // Проверка истечения срока действия токена
  getAuthHeaders: () => Record<string, string>;                 // Получение заголовков для API запросов
}

// Фабричный метод - возавращет хранилище авторизации

export const createAuthStore = (): AuthStore => {

 const { authApiUrl } = shopConfig

  const store =
  {
    user: null as User | null,
    isLoading: false,
    error: null as string | null,
    accessToken: localStorage.getItem('accessToken'),         // Восстанавливаем токен из localStorage
    refreshToken: localStorage.getItem('refreshToken'),       // Восстанавливаем refresh token из localStorage

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
        // запрос на аутентификацию
        const response = await fetch(`${authApiUrl}/login`, {
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

        // СОХРАНЯЕМ ПОЛУЧЕННЫЕ ТОКЕНЫ И ДАННЫЕ ПОЛЬЗОВАТЕЛЯ
        this.setTokens(data.data);

      }
      catch (error) {
        this.error = error instanceof Error ? error.message : 'Ошибка авторизации';
        console.error('Login error:', error);
      }
      finally {
        this.isLoading = false;
      }
    },

    async registration(data: RegistrationFormData): Promise<void> {
      this.isLoading = true;
      this.error = null;

      try {
        console.log(`${authApiUrl}register`);
        // запрос на регистрацию
        const response = await fetch(`${authApiUrl}register`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            name: data.name,
            email: data.email,
            password: data.password,
          }),
        });

        const responseData = await response.json();

        if (!response.ok) {
          const errorMessage = responseData.message ||
            (response.status === 409 ? 'Пользователь с таким email уже существует' : response.status === 400 ? 'Некорректные данные регистрации' : 'Ошибка регистрации');
          throw new Error(errorMessage);
        }

        // После успешной регистрации автоматически логиним пользователя
        if (responseData.data && responseData.data.accessToken) {
          this.setTokens(responseData.data);
        }
      }
      catch (error) {
        this.error = error instanceof Error ? error.message : 'Ошибка регистрации';
        console.error('Register error:', error);
      }
      finally {
        this.isLoading = false;
      }
    },

    // Обновляет access token с использованием refresh token
    // Вызывается автоматически при истечении срока действия access token
    async refreshTokens(): Promise<boolean> {

      if (!this.refreshToken || !this.accessToken) {
        this.logout(); // Если токенов нет - разлогиниваем пользователя
        return false;
      }

      try {
        const response = await fetch(`${authApiUrl}refresh`, {
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

        this.setTokens(data.data); return true;
      
      } catch (error) {
        console.error('Token refresh error:', error);
        this.logout();
        return false;
      }
    },

     // Выход пользователя из системы
     // Отправляет запрос на сервер для отзыва токенов и очищает локальные данные
    async logout(): Promise<void> {
      try {
        if (this.accessToken) {
          await fetch(`${authApiUrl}logout`, {
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

     // Инициализация состояния аутентификации при загрузке приложения
     // Восстанавливает пользователя из localStorage и проверяет валидность токенов
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
        // ЕСЛИ TOKEN ИСТЕК, НО ЕСТЬ REFRESH TOKEN - ПЫТАЕМСЯ ОБНОВИТЬ
        if (this.refreshToken) {
          this.refreshTokens().catch(console.error);
        } else {
          // ЕСЛИ REFRESH TOKEN НЕТ - ОЧИЩАЕМ ДАННЫЕ
          this.clearTokens();
        }
      }
    },

     // Проверяет валидность текущей аутентификации
     // Автоматически обновляет токены при необходимости
    async checkAuth(): Promise<boolean> {
      if (!this.accessToken) return false; 

      // ЕСЛИ TOKEN ИСТЕК - ПЫТАЕМСЯ ОБНОВИТЬ
      if (this.isTokenExpired()) {
        return await this.refreshTokens();
      }

      return true; // Token валиден
    },

     // Сохраняет токены и данные пользователя в store и localStorage
    setTokens(tokens: LoginResponseData): void {
      this.accessToken = tokens.accessToken;
      this.refreshToken = tokens.refreshToken;

      this.user = {
        id: tokens.userId,
        name: tokens.username,
        role: tokens.role as 'admin' | 'user',
        email: tokens.email,
        username: tokens.username
      };

      localStorage.setItem('accessToken', tokens.accessToken);
      localStorage.setItem('refreshToken', tokens.refreshToken);
      localStorage.setItem('tokenExpiry', tokens.expiresAt);
      localStorage.setItem('user', JSON.stringify(this.user));
    },

     // Очищает все данные аутентификации из store и localStorage
     // Вызывается при logout или при обнаружении невалидных данных
    clearTokens(): void {
      this.accessToken = null;
      this.refreshToken = null;
      this.user = null;

      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
      localStorage.removeItem('tokenExpiry');
      localStorage.removeItem('user');
    },

    // Проверяет, истек ли срок действия access token
    isTokenExpired(): boolean {
      try {
        const expiry = localStorage.getItem('tokenExpiry');
        if (!expiry) return true; // Если даты нет - считаем токен истекшим

        const expiryDate = new Date(expiry);
        // Добавляем запас в 1 минуту чтобы избежать ситуации, когда токен истекает во время запроса
        return Date.now() >= (expiryDate.getTime() - 60000);
      } catch {
        // При любой ошибке парсинга считаем токен истекшим
        return true;
      }
    },

     // Генерирует заголовки для HTTP запросов с авторизацией
    getAuthHeaders(): Record<string, string> {
      const headers: Record<string, string> = {
        'Content-Type': 'application/json' // Стандартный заголовок для JSON API
      };

      // ЕСЛИ ЕСТЬ ACCESS TOKEN - ДОБАВЛЯЕМ ЗАГОЛОВОК АВТОРИЗАЦИИ
      if (this.accessToken) {
        headers['Authorization'] = `Bearer ${this.accessToken}`;
      }
      return headers;
    },

  };

  return makeAutoObservable(store);
};

export default createAuthStore;