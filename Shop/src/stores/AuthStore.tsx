import { makeAutoObservable } from 'mobx';

export interface User {
  id: string;
  name: string;
  role: 'customer' | 'admin' | 'manager';
  email: string;
}

export interface AuthStore {
  user: User | null;
  isLoading: boolean;
  error: string | null;
  isAuthenticated: boolean;
  role: string;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
}

export const createAuthStore = (): AuthStore => {
  const store = {
    user: null as User | null,
    isLoading: false,
    error: null as string | null,

    // Геттеры
    get isAuthenticated(): boolean {
      return !!this.user;
    },

    get role(): string {
      return this.user?.role || 'guest';
    },

    // Методы
    async login(email: string, password: string): Promise<void> {
      this.isLoading = true;
      this.error = null;
      
      try {
        // Mock login - имитация API запроса
        await new Promise(resolve => setTimeout(resolve, 300));
        
        // Устанавливаем пользователя
        this.user = { 
          id: '1', 
          name: 'Клиент', 
          role: 'customer', 
          email 
        };
      } catch (error) {
        this.error = 'Ошибка авторизации';
        console.error('Login error:', error);
      } finally {
        this.isLoading = false;
      }
    },

    logout(): void {
      this.user = null;
      this.error = null;
    }
  };

  return makeAutoObservable(store);
};

export default createAuthStore;