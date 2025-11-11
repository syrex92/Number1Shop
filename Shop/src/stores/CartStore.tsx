import {makeAutoObservable, runInAction, toJS} from 'mobx';
import type {Product} from './ProductsStore';
import {CartApi, type CartItemResponseDto} from '../utils/CartApi';
import type createAuthStore from "./AuthStore.tsx";

export interface CartItem {
    product: Product;
    qty: number;
    toOrder: boolean;
}

export interface CartStore {
    items: Map<string, CartItem>;
    count: number;
    total: number;
    find: (productId: string) => CartItem | undefined;
    add: (product: Product) => Promise<CartItem>;
    remove: (productId: string) => void;
    orderTotalPrice: number;
    //orderTotalCount: number;
    
    toOrder: (productId: string, inOrder: boolean) => void;
    
    orderAll: boolean;
    decrease: (productId: string) => void;
    clear: () => void;
    
    fetchItems: () => Promise<void>;
    
    loading: boolean;
    error: boolean;
    errorText: string | null;
}

export const createCartStore = (auth: ReturnType<typeof createAuthStore>): CartStore => {
    
    const apiUrl = import.meta.env.VITE_SHOP_CART_URL;

    const cartApi = new CartApi(apiUrl, auth);
    
    const store = {
        
        items: new Map<string, CartItem>(),

        get orderAll():boolean {
            let c = true;
            this.items.forEach(v => (c &&= v.toOrder));
            return c;
        },
        
        get count(): number {
            let c = 0;
            this.items.forEach(v => (c += v.qty));
            return c;
        },

        get orderTotalPrice(): number {
            let c = 0;
            this.items.forEach(v => (c += (v.toOrder ? v.product.price * v.qty : 0)));
            return c;
        },
      
        get total(): number {
            let sum = 0;
            this.items.forEach(v => (sum += v.product.price * v.qty));
            return sum;
        },

        async fetchItems(): Promise<void> {

            
            
            runInAction(() => { 
                this.loading = true; 
                this.error = false; 
                this.errorText = ""; 
            })
            
            await cartApi.getItems()
                .then(async (dto) => {
                    this.loading = false;
                    
                    dto.items.forEach((x : CartItemResponseDto) => this.items.set(x.productId, 
                        {
                            toOrder: false, 
                            product: {
                                id: x.productId,
                                image: undefined,    
                                title: "title",
                                price: 100
                            }, 
                            qty: x.quantity
                        }));
                    
                })
                .catch(reason => {                    
                    runInAction(() => {
                        this.items.clear();
                        this.error = true;
                        this.errorText = reason.message;
                        this.loading = false;
                    })                    
                });
        },
        
        async add(product: Product): Promise<CartItem> {
            let rec = this.items.get(product.id);
            if (rec) {
                rec.qty += 1;
                await cartApi.updateItem(rec)
                    .then(() => {
                        //this.items.set(product.id, dto);
                        return rec;
                    })
                    .catch(reason => {
                        console.log(reason);
                    })
                //return rec;
            } else {
                rec = {product, qty: 1, toOrder: true};
                await cartApi.addItem(rec)
                    .then(dto => {
                        this.items.set(product.id, dto);
                        return rec;
                    })
                    .catch(reason => {
                        console.log(reason);
                    })
            }
            return toJS(rec);
        },

        remove(productId: string): void {
            this.items.delete(productId);
        },

        find(productId: string): CartItem | undefined {
            return this.items.get(productId);
        },

        decrease(productId: string): void {
            const rec = this.items.get(productId);
            if (!rec) return;
            rec.qty -= 1;
            if (rec.qty <= 0) this.items.delete(productId);
        },

        toOrder(productId: string, inOrder: boolean): void {
            const rec = this.items.get(productId);
            if (!rec) return;
            rec.toOrder = inOrder;
        },
        
        clear(): void {
            this.items.clear();
        },

        loading: false,
        error: false,
        errorText: ""
    };

    return makeAutoObservable(store);
};

export default createCartStore;