import GenericApi from "./GenericApi";
import type {CartItem} from "../stores/CartStore.tsx";
import {logger} from "./logger.ts";


export interface CartItemResponseDto {
    productId: string;
    quantity: number;
    toOrder: boolean;    
}

export interface CartResponseDto {
    items: CartItemResponseDto[];
    error: string | undefined;
}

export class CartApi extends GenericApi {

    //private static readonly apiUrl = import.meta.env.VITE_SHOP_CART_URL;

    // private getItemUrl(item: CartItem): string {
    //     return `${item.product.id}`;
    // }

    async getItems(): Promise<CartResponseDto | undefined> {
        
        try {
            const response = await this.get("");
            if (response.ok) {
                return response.json()
            }
            return undefined;
        }
        catch (error) {
            logger.error("CartApi getItems error", error);
            return undefined;
        }
        
        // return this.get("")
        //     .then(response => {
        //         if (response.ok) {
        //             return response.json()
        //         }
        //         throw new Error("Failed to get cart items: " + response.statusText);
        //     })
        //     .then(json => json as CartResponseDto);
    }

    async addItem(item: CartItem): Promise<CartItem | undefined> {
        try {
            const response: Response = await this.post("", { productId: item.product.id, quantity: item.qty });

            if(response.ok) {
                const data = await response.json() as CartItemResponseDto;
                item.qty = data.quantity;
                item.toOrder = data.toOrder;
                return item;                
            }
            return item;                        
        }
        catch (e) {
            console.error(e);
            return item;
        }
    }

    async updateItem(item: CartItem): Promise<CartItem | undefined> {
        try {
            const putData = { productId: item.product.id, quantity: item.qty };
            console.log("putData", putData);
            
            const response: Response = await this.put("", putData);
            if(response.ok) {

                const data = await response.json() as CartItemResponseDto;
                item.qty = data.quantity;
                item.toOrder = data.toOrder;
                console.log("putData result", item);
                return {...item, qty: data.quantity};
                
                //return response.json();
            }
            return item;            
        }
        catch (e) {
            console.error(e);
            return item;
        }
    }

    async deleteItem(item: CartItem): Promise<CartItem | undefined> {
        item.qty = 0;
        const putData = { productId: item.product.id, quantity: item.qty };
        const response: Response = await this.put("", putData);
        return response.json();
    }


}

export default CartApi;