import React, { createContext, useContext, useMemo, useEffect } from 'react';
import { enableStaticRendering } from 'mobx-react-lite';
import { createAuthStore } from '../stores/AuthStore';
import { createProductsStore } from '../stores/ProductsStore';
import { createCartStore } from '../stores/CartStore';
import { createFavoritesStore } from '../stores/FavoritesStore';
import { createOrdersStore } from '../stores/OrdersStore';
import { createNotificationStore } from '../stores/NotificationStore';

enableStaticRendering(typeof window === 'undefined');

interface RootStore {
  auth: ReturnType<typeof createAuthStore>;
  products: ReturnType<typeof createProductsStore>;
  cart: ReturnType<typeof createCartStore>;
  favorites: ReturnType<typeof createFavoritesStore>;
  orders: ReturnType<typeof createOrdersStore>;
  notifications: ReturnType<typeof createNotificationStore>;
}

const RootStoreContext = createContext<RootStore | null>(null);

export const RootStoreProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    
    const auth =  createAuthStore(); //createFakeAuthStore();     
    
  const rootStore = useMemo(() => {
    const cart = createCartStore(auth);

    const notifications = createNotificationStore(auth, (envelope) => {
      if (
        envelope.type === "stock.reservation_failed" ||
        envelope.type === "stock.confirm_failed"
      ) {
        const ids = Array.from(cart.items.values())
          .filter((item) => item.toOrder)
          .map((item) => item.product.id);
        if (ids.length > 0) {
          cart.markItemsUnavailable(ids);
        }
      }
    });

    return {
      auth,
      products: createProductsStore(auth),
      cart,
      favorites: createFavoritesStore(),
      orders: createOrdersStore(auth),
      notifications,
    };
  }, []);

  // Инициализируем аутентификацию при монтировании провайдера
  useEffect(() => {
    rootStore.auth.initializeAuth();
  }, [rootStore.auth]);

  return (
    <RootStoreContext.Provider value={rootStore}>
      {children}
    </RootStoreContext.Provider>
  );
};

export const useStores = (): RootStore => {
  const ctx = useContext(RootStoreContext);
  if (!ctx) {
    throw new Error('useStores must be used within a RootStoreProvider');
  }
  return ctx;
};

export default RootStoreContext;