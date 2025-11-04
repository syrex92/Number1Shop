import GenericApi from "./GenericApi";
import type {CartItem} from "../stores/CartStore.tsx";


export interface CartItemResponseDto {
    productId: string;
    quantity: number;
}

export interface CartResponseDto {
    items: CartItemResponseDto[];
}

export class CartApi extends GenericApi{
    
    private static readonly apiUrl = import.meta.env.VITE_SHOP_CART_URL;
        
    private getItemUrl(item: CartItem) : string {
        return `${CartApi.apiUrl}/${item.product.id}`;
    }
    
    async getItems(): Promise<CartResponseDto> {
        const response: Response = await this.get("");
        //console.log(response.body);
        return this.as<CartResponseDto>(response);
    }

    async addItem(item: CartItem): Promise<CartItem> {
        const response: Response = await this.post(CartApi.apiUrl, item);
        return this.as<CartItem>(response);
    }

    async updateItem(item: CartItem): Promise<CartItem> {
        const response: Response = await this.put(this.getItemUrl(item), item);
        return this.as<CartItem>(response);
    }

    async deleteItem(item: CartItem): Promise<CartItem> {
        const response: Response = await this.delete(this.getItemUrl(item));
        return this.as<CartItem>(response);
    }
    
    
}

export default CartApi;