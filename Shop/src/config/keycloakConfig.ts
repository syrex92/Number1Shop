// Shop/src/config/keycloakConfig.ts
import Keycloak from 'keycloak-js';
import shopConfig from './shopConfig';

// Мы используем keycloak только для работы с токенами, но НЕ для редиректов
const keycloak = new Keycloak({
  url: shopConfig.keycloakUrl,
  realm: 'shop',
  clientId: 'shop-ui',
});

// Отключаем авто-редиректы
keycloak.onAuthSuccess = undefined;
keycloak.onAuthError = undefined;

export default keycloak;