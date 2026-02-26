import { makeAutoObservable } from 'mobx';
import type createAuthStore from './AuthStore';
import shopConfig from '../config/shopConfig';

export interface OrderItem {
  id: string;
  name: string;
  quantity: number;
  cost: number;
}

export interface Order {
  id: string;
  orderNumber: number;
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
  cancelOrder: (id: string) => Promise<void>;
}

export const createOrdersStore = (_auth: ReturnType<typeof createAuthStore>): OrdersStore => {
  const { ordersApiUrl } = shopConfig

  const store = {
    orders: [] as Order[],
    isLoading: false,

    async fetchOrders(): Promise<void> {
      this.isLoading = true;
      try {
        if (this.orders.length > 0) {
          return;
        }
        const response = await fetch(ordersApiUrl, { headers: _auth.getAuthHeaders()}).then(function (response) {
          return response.json();
        })
        for (let i=0; i < response.data.length; i++) {
          this.orders.push(
            {
              id: response.data[i].id,
              orderNumber: response.data[i].orderNumber,
              createdAt: new Date(response.data[i].createdAt).toLocaleDateString('ru-RU', { day: '2-digit', month: '2-digit', year: 'numeric' }),
              deviveryAddress: response.data[i].deliveryAddress.postalCode + ' ' + response.data[i].deliveryAddress.country + ' ' + response.data[i].deliveryAddress.city +
                ' ' + response.data[i].deliveryAddress.street + ' ' + response.data[i].deliveryAddress.house + ' ' + response.data[i].deliveryAddress.appartment,
              status: response.data[i].status,
              totalPrice: response.data[i].cost,
              items: []
            }
          );
        }
      } finally {
        this.isLoading = false;
      }
    },

    async fetchOrderDetails(id: string): Promise<void> {
      this.isLoading = true;
      try {
        let index = this.orders.findIndex(o => o.id === id);
        if (index < 0) {
          await this.fetchOrders();
          index = this.orders.findIndex(o => o.id === id);
        }
        if (index > -1 && this.orders[index]?.items?.length > 0) {
          return;
        }
        const response = await fetch(ordersApiUrl + id, { headers: _auth.getAuthHeaders()}).then(function (response) {
          return response.json();
        })
        const order = this.orders[index];
        order.items = [];
        response.items.forEach((element: OrderItem) => {
          order.items.push(element);
        });
        this.orders[index] = { ...order };
      } finally {
        this.isLoading = false;
      }
    },

    async cancelOrder(id: string): Promise<void> {
      this.isLoading = true;
      try {
        const index = this.orders.findIndex(o => o.id === id);
        const response = await fetch(ordersApiUrl + id, { method: 'PATCH', headers: _auth.getAuthHeaders(), body: JSON.stringify({ status: 'Cancelled' }) });
        if (response.ok) {
          const order = this.orders[index];
          order.status = 'Cancelled';
          this.orders[index] = { ...order };
        } else {
          alert(response.statusText);
        }
      } finally {
        this.isLoading = false;
      }
    }
  };

  return makeAutoObservable(store);
};

export default createOrdersStore;