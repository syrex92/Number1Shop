import { makeAutoObservable } from 'mobx';

export interface Order {
  id: string;
  // можно добавить поля заказа при необходимости
}

export interface OrdersStore {
  orders: Order[];
  isLoading: boolean;
  fetchOrders: () => Promise<void>;
}

export const createOrdersStore = (): OrdersStore => {
  const store = {
    orders: [] as Order[],
    isLoading: false,

    async fetchOrders(): Promise<void> {
      this.isLoading = true;
      try {
        await new Promise(resolve => setTimeout(resolve, 200));
        // Mock data
        this.orders = [];
      } finally {
        this.isLoading = false;
      }
    }
  };

  return makeAutoObservable(store);
};

export default createOrdersStore;