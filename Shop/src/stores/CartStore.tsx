import {makeAutoObservable, toJS} from 'mobx';
import type {Product} from './ProductsStore';

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
    add: (product: Product) => CartItem;
    remove: (productId: string) => void;
    orderTotalPrice: number;
    //orderTotalCount: number;
    
    orderAll: boolean;
    decrease: (productId: string) => void;
    clear: () => void;
    
}

export const createCartStore = (): CartStore => {
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
            this.items.forEach(v => (c += (v.toOrder ? v.qty : 0)));
            return c;
        },
      
        get total(): number {
            let sum = 0;
            this.items.forEach(v => (sum += v.product.price * v.qty));
            return sum;
        },

        add(product: Product): CartItem {
            let rec = this.items.get(product.id);
            if (rec) {
                rec.qty += 1;
            } else {
                rec = {product, qty: 1, toOrder: true};
                this.items.set(product.id, rec);
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

        clear(): void {
            this.items.clear();
        }
    };

    return makeAutoObservable(store);
};

export default createCartStore;