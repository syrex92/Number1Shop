import GenericApi from "./GenericApi";
import type {CartItem} from "../stores/CartStore.tsx";


export interface CartItemResponseDto {
    productId: string;
    quantity: number;
}

export interface CartResponseDto {
    items: CartItemResponseDto[];
    error: string | undefined;
}

export class CartApi extends GenericApi {

    //private static readonly apiUrl = import.meta.env.VITE_SHOP_CART_URL;

    private getItemUrl(item: CartItem): string {
        return `${item.product.id}`;
    }

    async getItems(): Promise<CartResponseDto> {
        return this.get("")
            .then(response => {
                if (response.ok) {
                    return response.json()
                }
                throw new Error("Failed to get cart items: " + response.statusText);
            })
            .then(json => json as CartResponseDto);
    }

    async addItem(item: CartItem): Promise<CartItem> {
        const response: Response = await this.post("", { productId: item.product.id, quantity: item.qty });
        return this.as<CartItem>(response);
    }

    async updateItem(item: CartItem): Promise<CartItem> {
        const response: Response = await this.put("", { productId: item.product.id, quantity: item.qty });
        return this.as<CartItem>(response);
    }

    async deleteItem(item: CartItem): Promise<CartItem> {
        const response: Response = await this.delete(this.getItemUrl(item));
        return this.as<CartItem>(response);
    }


}

export default CartApi;