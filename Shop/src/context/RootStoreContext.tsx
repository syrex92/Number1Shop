import React, { createContext, useContext, useMemo } from 'react';
import { enableStaticRendering } from 'mobx-react-lite';
import { createAuthStore } from '../stores/AuthStore';
import { createProductsStore } from '../stores/ProductsStore';
import { createCartStore } from '../stores/CartStore';
import { createFavoritesStore } from '../stores/FavoritesStore';
import { createOrdersStore } from '../stores/OrdersStore';

enableStaticRendering(typeof window === 'undefined');

interface RootStore {
  auth: ReturnType<typeof createAuthStore>;
  products: ReturnType<typeof createProductsStore>;
  cart: ReturnType<typeof createCartStore>;
  favorites: ReturnType<typeof createFavoritesStore>;
  orders: ReturnType<typeof createOrdersStore>;
}

const RootStoreContext = createContext<RootStore | null>(null);

export const RootStoreProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const value = useMemo(() => ({
    auth: createAuthStore(),
    products: createProductsStore(),
    cart: createCartStore(),
    favorites: createFavoritesStore(),
    orders: createOrdersStore()
  }), []);

  return (
    <RootStoreContext.Provider value={value}>
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