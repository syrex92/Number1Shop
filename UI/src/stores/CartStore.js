import { makeAutoObservable } from 'mobx';

export class CartStore {
  items = new Map(); // productId -> { product, qty }

  constructor() {
    makeAutoObservable(this, {}, { autoBind: true });
  }

  get count() {
    let c = 0;
    this.items.forEach(v => (c += v.qty));
    return c;
  }

  get total() {
    let sum = 0;
    this.items.forEach(v => (sum += v.product.price * v.qty));
    return sum;
  }

  add(product) {
    const rec = this.items.get(product.id);
    if (rec) {
      rec.qty += 1;
    } else {
      this.items.set(product.id, { product, qty: 1 });
    }
  }

  remove(productId) {
    this.items.delete(productId);
  }

  decrease(productId) {
    const rec = this.items.get(productId);
    if (!rec) return;
    rec.qty -= 1;
    if (rec.qty <= 0) this.items.delete(productId);
  }

  clear() {
    this.items.clear();
  }
}

export default CartStore;


