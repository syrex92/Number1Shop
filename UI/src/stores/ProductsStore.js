import { makeAutoObservable } from 'mobx';

export class ProductsStore {
  products = [];
  isLoading = false;
  query = '';

  constructor() {
    makeAutoObservable(this, {}, { autoBind: true });
  }

  get filteredProducts() {
    const q = this.query.trim().toLowerCase();
    if (!q) return this.products;
    return this.products.filter(p => p.title.toLowerCase().includes(q));
  }

  setQuery(value) {
    this.query = value;
  }

  async fetchProducts() {
    this.isLoading = true;
    try {
      await new Promise(r => setTimeout(r, 300));
      // Mock data
      this.products = [
        { id: 'p1', title: 'Ноутбук', price: 70000, image: '/logo192.png' },
        { id: 'p2', title: 'Смартфон', price: 40000, image: '/logo192.png' },
        { id: 'p3', title: 'Наушники', price: 5000, image: '/logo192.png' },
      ];
    } finally {
      this.isLoading = false;
    }
  }
}

export default ProductsStore;


