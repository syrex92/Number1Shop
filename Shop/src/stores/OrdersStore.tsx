import { makeAutoObservable } from 'mobx';

export interface OrderItem {
  id: string;
  name: string;
  quantity: number;
}

export interface Order {
  id: string;
  createdAt: string;
  deviveryAddress: string;
  status: string;
  totalPrice: number;
  items: OrderItem[];
}

export interface OrdersStore {
  orders: Order[];
  isLoading: boolean;
  fetchOrders: () => Promise<void>;
  fetchOrderDetails: (id: string) => Promise<void>;
}

export const createOrdersStore = (): OrdersStore => {
  const store = {
    orders: [] as Order[],
    isLoading: false,

    async fetchOrders(): Promise<void> {
      this.isLoading = true;
      try {
        if (this.orders.length > 0) {
          return;
        }
        await new Promise(resolve => setTimeout(resolve, 200));
        // Mock data
        this.orders = [
          { id: '1', createdAt: '01.01.2023', deviveryAddress: '123 Main St', status: 'Delivered', totalPrice: 150.00, items: [] },
          { id: '2', createdAt: '15.02.2023', deviveryAddress: '456 Oak Ave', status: 'Processing', totalPrice: 85.50, items: [] },
          { id: '3', createdAt: '03.10.2023', deviveryAddress: '789 Pine Rd', status: 'Cancelled', totalPrice: 42.75, items: [] },
        ];
      } finally {
        this.isLoading = false;
      }
    },

    async fetchOrderDetails(id: string): Promise<void> {
      this.isLoading = true;
      try {
        const index = this.orders.findIndex(o => o.id === id);
        if (this.orders[index]?.items?.length > 0) {
          return;
        }
        await new Promise(resolve => setTimeout(resolve, 200));
        // Mock data
        const orders = [
          { id: '1', createdAt: '01.01.2023', deviveryAddress: '123 Main St', status: 'Delivered', totalPrice: 150.00, items: [
            { id: 'a', name: 'Product A', quantity: 2 },
            { id: 'b', name: 'Product B', quantity: 1 },
          ] },
          { id: '2', createdAt: '15.02.2023', deviveryAddress: '456 Oak Ave', status: 'Processing', totalPrice: 85.50, items: [
            { id: 'c', name: 'Product C', quantity: 3 },
          ] },
          { id: '3', createdAt: '03.10.2023', deviveryAddress: '789 Pine Rd', status: 'Cancelled', totalPrice: 42.75, items: [
            { id: 'd', name: 'Product D', quantity: 1}
          ] },
        ];
        const order = orders.find(o => o.id === id);
        if (order) {          
          if (index >= 0) {
            this.orders[index] = order;
          } else {
            this.orders.push(order);
          }
        }
      } finally {
        this.isLoading = false;
      }
    }
  };

  return makeAutoObservable(store);
};

export default createOrdersStore;