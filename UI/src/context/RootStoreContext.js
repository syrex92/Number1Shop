import React, { createContext, useContext, useMemo } from 'react';
import { enableStaticRendering } from 'mobx-react-lite';
import AuthStore from '../stores/AuthStore';
import ProductsStore from '../stores/ProductsStore';
import CartStore from '../stores/CartStore';
import FavoritesStore from '../stores/FavoritesStore';
import OrdersStore from '../stores/OrdersStore';

enableStaticRendering(false);

const RootStoreContext = createContext(null);

export const RootStoreProvider = ({ children }) => {
  const value = useMemo(() => {
    const auth = new AuthStore();
    const products = new ProductsStore();
    const cart = new CartStore();
    const favorites = new FavoritesStore();
    const orders = new OrdersStore();
    return { auth, products, cart, favorites, orders };
  }, []);

  return (
    <RootStoreContext.Provider value={value}>{children}</RootStoreContext.Provider>
  );
};

export const useStores = () => {
  const ctx = useContext(RootStoreContext);
  if (!ctx) throw new Error('RootStoreContext не инициализирован');
  return ctx;
};

export default RootStoreContext;


