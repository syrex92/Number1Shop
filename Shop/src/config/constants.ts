// Конфигурационные константы приложения
import shopConfig from "./shopConfig.ts";

const {authApiUrl, rootApiUrl} = shopConfig

export const API_BASE_URL = rootApiUrl; //'https://your-api-domain.com/api';

export const API_AUTH_URL = authApiUrl; //'https://localhost:7294/api/auth';
export const APP_NAME = 'Number One Shop';
export const APP_VERSION = '1.0.0';