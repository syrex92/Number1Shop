/// <reference types="vite/client" />

interface ImportMetaEnv {
    readonly VITE_SHOP_SITE_URL: string;
    readonly VITE_SHOP_ROOT_API_URL: string;
    readonly VITE_SHOP_CART_API_URL: string;
    readonly VITE_SHOP_FAV_API_URL: string;
    readonly VITE_SHOP_CATALOG_API_URL: string;
    readonly VITE_SHOP_USERS_API_URL: string;
    readonly VITE_SHOP_AUTH_API_URL: string;
    readonly VITE_SHOP_ORDERS_API_URL: string;
    readonly VITE_SHOP_STORAGE_API_URL: string;
    readonly VITE_SHOP_IMAGES_URL: string;
}

interface ImportMeta {
    readonly env: ImportMetaEnv;
}
