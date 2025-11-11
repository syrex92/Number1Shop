//import { createAuthStore } from '../stores/AuthStore';
import { API_BASE_URL } from '../config/constants';
import sleep from "./sleep.ts";
import type createAuthStore from "../stores/AuthStore.tsx";

/**
 * Интерфейс для HTTP заголовков
 */
interface RequestHeaders {
    [key: string]: string; 
}

/**
 * Класс API клиента для выполнения HTTP запросов
 * Автоматически добавляет JWT токены и обновляет их при истечении
 */
class ApiClient {
    /**
     * Базовый URL API сервера
     */
    private baseURL: string;

    /**
     * Ссылка на хранилище аутентификации
     * Используется для получения токенов и их автоматического обновления
     */
    private authStore: ReturnType<typeof createAuthStore>;

    /**
     * Конструктор API клиента
     * @param baseURL - Базовый URL API сервера
     * @param authStore - Экземпляр хранилища аутентификации
     */
    constructor(baseURL: string, authStore: ReturnType<typeof createAuthStore>) {
        this.baseURL = baseURL;
        this.authStore = authStore;
    }

    /**
     * Основной метод для выполнения HTTP запросов
     * Автоматически добавляет заголовки авторизации и обрабатывает истекшие токены
     */
    async request(url: string, options: RequestInit = {}): Promise<Response> {
        await sleep(3000);
        // ПРЕДВАРИТЕЛЬНАЯ ПРОВЕРКА АУТЕНТИФИКАЦИИ
        const isAuthenticated = await this.authStore.checkAuth();

        // СОЗДАЕМ ЗАГОЛОВКИ ЗАПРОСА
        // Объединяем стандартные заголовки авторизации с пользовательскими
        const headers: RequestHeaders = {
            ...this.authStore.getAuthHeaders(), // Добавляем Content-Type и Authorization
            ...(options.headers as RequestHeaders), // Пользовательские заголовки имеют приоритет
        };

        //console.log(headers);
        //console.log(`Requesting from ${this.baseURL}${url}`);
        // ВЫПОЛНЯЕМ ПЕРВОНАЧАЛЬНЫЙ ЗАПРОС
        let response = await fetch(`${this.baseURL}${url}`, {
            ...options, // Копируем все опции (method, body, credentials, etc.)
            headers,    // Передаем сформированные заголовки
        });

        // ОБРАБАТЫВАЕМ СЛУЧАЙ ИСТЕЧЕНИЯ TOKEN'A
        // Если сервер вернул 401 (Unauthorized) и пользователь был аутентифицирован
        if (response.status === 401 && isAuthenticated) {
            // ПЫТАЕМСЯ ОБНОВИТЬ ТОКЕНЫ
            const refreshed = await this.authStore.refreshTokens();
            
            // ЕСЛИ ТОКЕНЫ УСПЕШНО ОБНОВЛЕНЫ - ПОВТОРЯЕМ ЗАПРОС
            if (refreshed && this.authStore.accessToken) {
                // ОБНОВЛЯЕМ ЗАГОЛОВОК АВТОРИЗАЦИИ С НОВЫМ ТОКЕНОМ
                headers['Authorization'] = `Bearer ${this.authStore.accessToken}`;
                
                // ПОВТОРЯЕМ ОРИГИНАЛЬНЫЙ ЗАПРОС С ОБНОВЛЕННЫМИ ЗАГОЛОВКАМИ
                response = await fetch(`${this.baseURL}${url}`, {
                    ...options,
                    headers,
                });
            }
        }
        return response;
    }

    /**
     * Выполняет GET запрос для получения данных
     */
    async get(url: string): Promise<Response> {
        return this.request(url, { method: 'GET' });
    }

    /**
     * Выполняет POST запрос для создания новых данных
     */
    async post(url: string, data?: any): Promise<Response> {
        return this.request(url, {
            method: 'POST',
            body: data ? JSON.stringify(data) : undefined, // Преобразуем объект в JSON строку
        });
    }

    /**
     * Выполняет PUT запрос для полного обновления данных
     */
    async put(url: string, data?: any): Promise<Response> {
        return this.request(url, {
            method: 'PUT',
            body: data ? JSON.stringify(data) : undefined,
        });
    }

    /**
     * Выполняет DELETE запрос для удаления данных
     */
    async delete(url: string): Promise<Response> {
        return this.request(url, { method: 'DELETE' });
    }
}

/**
 * Фабричная функция для создания экземпляра API клиента
 */
export const createApiClient = (authStore: ReturnType<typeof createAuthStore>) => {
    return new ApiClient(API_BASE_URL, authStore);
};

export default ApiClient;