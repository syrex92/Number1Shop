import {makeAutoObservable, runInAction} from 'mobx';
import type {ProductItemResponseDto} from "../dto/ProductItemResponseDto.ts";

export interface Product {
  id: string;
  title: string;
  price: number;
  image: string | undefined;  
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
        runInAction(() => { this.isLoading = true; });

        console.log("Start products fetching");

        fetch("http://localhost/api/v1/catalog/products")
            .then(res => res.json())
            .then(async (res) => {
                
                const loadedItems = res as ProductItemResponseDto[];

                //console.log(loadedItems);

                const products = new Array<Product>(); 
                loadedItems.forEach((x : ProductItemResponseDto) => products.push(
                    {
                            id: x.id,
                            image: ((x.imagesUrls.length > 0) ? `/images/${x.imagesUrls[0]}` : undefined),                            
                            title: x.productTitle,
                            price: x.price,

                        // stockQuantity: number;
                        // productTitle: string;
                        // productDescription: string;
                        // productCategory: string;
                        //
                        // imagesUrls: string[];
                        
                    }));
                
                runInAction(() => { this.products = products; });

                console.log("Products loaded");
            })
            .catch(reason => {
                console.log("Products fetch error");
                console.log(reason);                
            })
            .finally(() => {
                runInAction(() => { this.isLoading = false; });
            });
      
      // try {
      //    
      //   await new Promise(resolve => setTimeout(resolve, 300));
      //   // Mock data
      //   this.products = [
      //     { id: 'p1', title: 'Ноутбук', price: 70000, image: '/logo192.png' },
      //     { id: 'p2', title: 'Смартфон', price: 40000, image: '/logo192.png' },
      //     { id: 'p3', title: 'Наушники', price: 5000, image: '/logo192.png' },
      //   ];
      // } finally {
      //   this.isLoading = false;
      // }
    }
  };

  return makeAutoObservable(store);
};

export default createProductsStore;