import { makeAutoObservable } from 'mobx';

export class OrdersStore {
  orders = [];
  isLoading = false;

  constructor() {
    makeAutoObservable(this, {}, { autoBind: true });
  }

  async fetchOrders() {
    this.isLoading = true;
    try {
      await new Promise(r => setTimeout(r, 200));
      // Mock
      this.orders = [];
    } finally {
      this.isLoading = false;
    }
  }
}

export default OrdersStore;


