import { makeAutoObservable } from 'mobx';
import { API_BASE_URL } from '../config/constants';
import type {AuthStore, LoginResponseData, User} from "./AuthStore.tsx";
import type { RegistrationFormData } from '../components/registration/RegistrationFormFields';

/**
 * Интерфейс хранилища аутентификации
 */
export interface FakeAuthStore {
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
  refreshTokens: () => Promise<boolean>;                        // Обновление токенов
  initializeAuth: () => void;                                   // Инициализация при загрузке приложения
  checkAuth: () => Promise<boolean>;                            // Проверка валидности аутентификации

  // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ
  setTokens: (tokens: LoginResponseData) => void;               // Сохранение токенов и данных пользователя
  clearTokens: () => void;                                      // Очистка токенов и данных пользователя
  isTokenExpired: () => boolean;                                // Проверка истечения срока действия токена
  getAuthHeaders: () => Record<string, string>;                // Получение заголовков для API запросов
}

/**
 * Фабричный метод - возавращет хранилище авторизации
 */
export const createFakeAuthStore = (): AuthStore => {
  const store = {
    user: null as User | null,
    isLoading: false,
    error: null as string | null,
      
    accessToken: null as string | null,
    refreshToken: null as string | null,

    get isAuthenticated(): boolean {
      return !!this.accessToken && !this.isTokenExpired();
    },

    get role(): string {
      return this.user?.role || 'guest';
    },

    async login(email: string, password: string): Promise<void> {
      this.isLoading = true;
      this.error = null;

      console.log(`Fake login with email: ${email} and password: ${password}`);
      
      try {
        
        // СОХРАНЯЕМ ПОЛУЧЕННЫЕ ТОКЕНЫ И ДАННЫЕ ПОЛЬЗОВАТЕЛЯ
        this.setTokens();

      }
      catch (error) {
        this.error = error instanceof Error ? error.message : 'Ошибка авторизации';
        console.error('Login error:', error);
      }
      finally {
        this.isLoading = false;
      }
    },

    /**
     * Обновляет access token с использованием refresh token
     * Вызывается автоматически при истечении срока действия access token
     * @returns true если токены успешно обновлены, false в случае ошибки
     */
    async refreshTokens(): Promise<boolean> {
      // ПРОВЕРЯЕМ НАЛИЧИЕ ТОКЕНОВ ДЛЯ ОБНОВЛЕНИЯ
      if (!this.refreshToken || !this.accessToken) {
        this.logout(); // Если токенов нет - разлогиниваем пользователя
        return false;
      }

      try {
        // ОТПРАВЛЯЕМ ЗАПРОС НА ОБНОВЛЕНИЕ ТОКЕНОВ
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

        // ПРОВЕРЯЕМ УСПЕШНОСТЬ ОБНОВЛЕНИЯ
        if (!response.ok || !data.success) {
          throw new Error(data.message || 'Failed to refresh token');
        }

        // СОХРАНЯЕМ НОВЫЕ ТОКЕНЫ
        this.setTokens();
        return true;
      } catch (error) {
        // ПРИ ОШИБКЕ ОБНОВЛЕНИЯ - РАЗЛОГИНИВАЕМ ПОЛЬЗОВАТЕЛЯ
        console.error('Token refresh error:', error);
        this.logout();
        return false;
      }
    },

    async registration(data: RegistrationFormData): Promise<void> {
          this.isLoading = true;
          this.error = null;
    
          try {
            // запрос на регистрацию
            const response = await fetch(`${API_BASE_URL}/register`, {
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
              this.setTokens();
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

    /**
     * Выход пользователя из системы
     * Отправляет запрос на сервер для отзыва токенов и очищает локальные данные
     */
    async logout(): Promise<void> {
      try {
        // ЕСЛИ ЕСТЬ ACCESS TOKEN - ОТПРАВЛЯЕМ ЗАПРОС НА СЕРВЕР ДЛЯ LOGOUT
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

    /**
     * Инициализация состояния аутентификации при загрузке приложения
     * Восстанавливает пользователя из localStorage и проверяет валидность токенов
     */
    initializeAuth(): void {
      // ВОССТАНАВЛИВАЕМ ДАННЫЕ ПОЛЬЗОВАТЕЛЯ ИЗ LOCALSTORAGE
      const savedUser = localStorage.getItem('user');
      if (savedUser) {
        try {
          this.user = JSON.parse(savedUser);
        } catch (error) {
          console.error('Error parsing saved user:', error);
          this.clearTokens(); // При ошибке парсинга - очищаем данные
        }
      }

      // ПРОВЕРЯЕМ ВАЛИДНОСТЬ ACCESS TOKEN
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

    /**
     * Проверяет валидность текущей аутентификации
     * Автоматически обновляет токены при необходимости
     * @returns true если пользователь аутентифицирован, false если нет
     */
    async checkAuth(): Promise<boolean> {
      if (!this.accessToken) return false; // Если токена нет - не аутентифицирован

      // ЕСЛИ TOKEN ИСТЕК - ПЫТАЕМСЯ ОБНОВИТЬ
      if (this.isTokenExpired()) {
        return await this.refreshTokens();
      }

      return true; // Token валиден
    },

    /**
     * Сохраняет токены и данные пользователя в store и localStorage
     * @param tokens - Данные ответа от сервера с токенами и информацией о пользователе
     */
    setTokens(): void {
      console.log("Set fake token");
      
      this.accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IkpvaG4iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zaWQiOiIxZjIyMTEzOC1kNmE0LTQ3ZDMtODM5MS02ZTFiYzlkNWIyZGUiLCJuYmYiOjE3NjE4NTc1MTQsImV4cCI6MTc2MTg1OTMxNCwiaWF0IjoxNzYxODU3NTE0fQ.yWEOLsIeZRKB7Cg39g6nTllHri_zxMCezdbsHvUMclc";
      this.refreshToken = "";

      // СОЗДАЕМ ОБЪЕКТ ПОЛЬЗОВАТЕЛЯ
      this.user = {
        id: "tokens.userId", 
        name: "tokens.username",
        role: 'user',
        email: "tokens.email",
        username: "tokens.username"
      };

      // СОХРАНЯЕМ ДАННЫЕ В LOCALSTORAGE ДЛЯ ВОССТАНОВЛЕНИЯ ПРИ ПЕРЕЗАГРУЗКЕ
      localStorage.setItem('accessToken', this.accessToken);
      localStorage.setItem('refreshToken', this.refreshToken);
      //localStorage.setItem('tokenExpiry', tokens.expiresAt);
      localStorage.setItem('user', JSON.stringify(this.user));
    },

    /**
     * Очищает все данные аутентификации из store и localStorage
     * Вызывается при logout или при обнаружении невалидных данных
     */
    clearTokens(): void {
      // ОЧИЩАЕМ STORE
      this.accessToken = null;
      this.refreshToken = null;
      this.user = null;

      // ОЧИЩАЕМ LOCALSTORAGE
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
      localStorage.removeItem('tokenExpiry');
      localStorage.removeItem('user');
    },

    /**
     * Проверяет, истек ли срок действия access token
     * @returns true если токен истек, false если еще валиден
     */
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

    /**
     * Генерирует заголовки для HTTP запросов с авторизацией
     * @returns Объект с заголовками для fetch/axios запросов
     */
    getAuthHeaders(): Record<string, string> {
      const headers: Record<string, string> = {
        'Content-Type': 'application/json' // Стандартный заголовок для JSON API
      };

        this.accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IkpvaG4iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zaWQiOiIxZjIyMTEzOC1kNmE0LTQ3ZDMtODM5MS02ZTFiYzlkNWIyZGUiLCJuYmYiOjE3NjIwNzU5MTAsImV4cCI6MTc2NDY2NzkxMCwiaWF0IjoxNzYyMDc1OTEwfQ.J1KpOGIbgMhbFBxXKoW9NVa2dnyHtdL6G5__ODCR3dM";
        
      console.log("GET AUTH HEADER")
      
      // ЕСЛИ ЕСТЬ ACCESS TOKEN - ДОБАВЛЯЕМ ЗАГОЛОВОК АВТОРИЗАЦИИ
      if (this.accessToken) {
        console.log("SET TOKEN")
        headers['Authorization'] = `Bearer ${this.accessToken}`;
      }
      return headers;
    },

  };

  // ДЕЛАЕМ ВЕСЬ STORE НАБЛЮДАЕМЫМ ДЛЯ MOBX
  // MobX БУДЕТ АВТОМАТИЧЕСКИ ОТСЛЕЖИВАТЬ ИЗМЕНЕНИЯ И УВЕДОМЛЯТЬ REACT КОМПОНЕНТЫ
  return makeAutoObservable(store);
};

export default createFakeAuthStore;