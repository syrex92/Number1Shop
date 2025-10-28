import { makeAutoObservable } from 'mobx';

export interface Product {
  id: string;
  title: string;
  price: number;
  image: string;  
}

export interface ProductsStore {
  products: Product[];
  isLoading: boolean;
  query: string;
  filteredProducts: Product[];
  setQuery: (value: string) => void;
  fetchProducts: () => Promise<void>;
}

export const createProductsStore = (): ProductsStore => {
  const store = {
    products: [] as Product[],
    isLoading: false,
    query: '',

    get filteredProducts(): Product[] {
      const q = this.query.trim().toLowerCase();
      if (!q) return this.products;
      return this.products.filter(p => p.title.toLowerCase().includes(q));
    },

    setQuery(value: string): void {
      this.query = value;
    },

    async fetchProducts(): Promise<void> {
      this.isLoading = true;
      try {
        await new Promise(resolve => setTimeout(resolve, 300));
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
  };

  return makeAutoObservable(store);
};

export default createProductsStore;