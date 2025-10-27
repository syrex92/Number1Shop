import { createAuthStore } from '../stores/AuthStore';
import { API_BASE_URL } from '../config/constants';

// Интерфейс для заголовков
interface RequestHeaders {
    [key: string]: string;
}

class ApiClient {
    private baseURL: string;
    private authStore: ReturnType<typeof createAuthStore>;

    constructor(baseURL: string, authStore: ReturnType<typeof createAuthStore>) {
        this.baseURL = baseURL;
        this.authStore = authStore;
    }

    async request(url: string, options: RequestInit = {}): Promise<Response> {
        // Проверяем аутентификацию перед запросом
        const isAuthenticated = await this.authStore.checkAuth();

        // Создаем заголовки с правильной типизацией
        const headers: RequestHeaders = {
            ...this.authStore.getAuthHeaders(),
            ...(options.headers as RequestHeaders),
        };

        let response = await fetch(`${this.baseURL}${url}`, {
            ...options,
            headers,
        });

        // Если токен истек во время запроса - пытаемся обновить и повторяем запрос
        if (response.status === 401 && isAuthenticated) {
            const refreshed = await this.authStore.refreshTokens();
            if (refreshed && this.authStore.accessToken) {
                // Обновляем токен в заголовках и повторяем запрос
                headers['Authorization'] = `Bearer ${this.authStore.accessToken}`;
                response = await fetch(`${this.baseURL}${url}`, {
                    ...options,
                    headers,
                });
            }
        }

        return response;
    }

    async get(url: string): Promise<Response> {
        return this.request(url, { method: 'GET' });
    }

    async post(url: string, data?: any): Promise<Response> {
        return this.request(url, {
            method: 'POST',
            body: data ? JSON.stringify(data) : undefined,
        });
    }

    async put(url: string, data?: any): Promise<Response> {
        return this.request(url, {
            method: 'PUT',
            body: data ? JSON.stringify(data) : undefined,
        });
    }

    async delete(url: string): Promise<Response> {
        return this.request(url, { method: 'DELETE' });
    }
}

// Создаем экземпляр API клиента
export const createApiClient = (authStore: ReturnType<typeof createAuthStore>) => {
    return new ApiClient(API_BASE_URL, authStore);
};

export default ApiClient;