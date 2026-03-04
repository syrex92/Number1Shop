import addEndSlash from "../utils/stringUtils.ts";

export interface IShopConfig {
    /** Базовый адрес сайта (http://server/) */
    siteUrl: string;

    /** Базовый адрес API (http://server/api/v1/) */
    rootApiUrl: string;

    /** Адрес API корзины (http://server/api/v1/cart/) */
    cartApiUrl: string;

    /** Адрес API избранного (http://server/api/v1/fav/) */
    favApiUrl: string;

    /** Адрес API каталога (http://server/api/v1/catalog/) */
    catalogApiUrl: string;

    /** Адрес API пользователей (http://server/api/v1/users/) */
    usersApiUrl: string;

    /** Адрес API авторизации (http://server/api/v1/auth/) */
    authApiUrl: string;

    /** Адрес API хранилища (http://server/api/v1/storage/) */
    storageApiUrl: string;

    /** Адрес API заказов (http://server/api/v1/orders/) */
    ordersApiUrl: string;
    
    /** Адрес (базовый) хранилища картинок (http://server/images/) */
    imagesUrl: string;
}

const shopConfig : IShopConfig = {
    siteUrl: addEndSlash(import.meta.env.VITE_SHOP_SITE_URL),
    rootApiUrl: addEndSlash(import.meta.env.VITE_SHOP_ROOT_API_URL),
    cartApiUrl: addEndSlash(import.meta.env.VITE_SHOP_CART_API_URL),    
    favApiUrl: addEndSlash(import.meta.env.VITE_SHOP_FAV_API_URL),
    catalogApiUrl: addEndSlash(import.meta.env.VITE_SHOP_CATALOG_API_URL),
    usersApiUrl: addEndSlash(import.meta.env.VITE_SHOP_USERS_API_URL),
    authApiUrl: addEndSlash(import.meta.env.VITE_SHOP_AUTH_API_URL),
    storageApiUrl: addEndSlash(import.meta.env.VITE_SHOP_STORAGE_API_URL),
    ordersApiUrl: addEndSlash(import.meta.env.VITE_SHOP_ORDERS_API_URL),
    imagesUrl: addEndSlash(import.meta.env.VITE_SHOP_IMAGES_URL)
}

export default shopConfig;