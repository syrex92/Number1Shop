import { makeAutoObservable } from 'mobx';
import type { Product } from './ProductsStore';

export interface CartItem {
  product: Product;
  qty: number;
}

export interface CartStore {
  items: Map<string, CartItem>;
  count: number;
  total: number;
  add: (product: Product) => void;
  remove: (productId: string) => void;
  decrease: (productId: string) => void;
  clear: () => void;
}

export const createCartStore = (): CartStore => {
  const store = {
    items: new Map<string, CartItem>(),

    get count(): number {
      let c = 0;
      this.items.forEach(v => (c += v.qty));
      return c;
    },

    get total(): number {
      let sum = 0;
      this.items.forEach(v => (sum += v.product.price * v.qty));
      return sum;
    },

    add(product: Product): void {
      const rec = this.items.get(product.id);
      if (rec) {
        rec.qty += 1;
      } else {
        this.items.set(product.id, { product, qty: 1 });
      }
    },

    remove(productId: string): void {
      this.items.delete(productId);
    },

    decrease(productId: string): void {
      const rec = this.items.get(productId);
      if (!rec) return;
      rec.qty -= 1;
      if (rec.qty <= 0) this.items.delete(productId);
    },

    clear(): void {
      this.items.clear();
    }
  };

  return makeAutoObservable(store);
};

export default createCartStore;