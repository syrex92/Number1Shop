import {makeAutoObservable, runInAction, toJS} from "mobx";
import {type Product, type ProductItemResponseDto} from "./ProductsStore";
import {CartApi} from "../utils/CartApi";
import type createAuthStore from "./AuthStore.tsx";
import shopConfig from "../config/shopConfig.ts";
import {logger} from "../utils/logger.ts";

//import {logger} from "../utils/logger.ts";

export interface CartItem {
    product: Product;
    productId: string;
    qty: number;
    toOrder: boolean;
}

export interface CartStore {
    items: Map<string, CartItem>;
    count: number;
    total: number;
    find: (productId: string) => CartItem | undefined;
    add: (product: Product) => Promise<CartItem>;
    // addWithQuantity: (
    //     product: Product,
    //     quantity: number,
    // ) => Promise<CartItem | undefined>;
    remove: (productId: string, all: boolean) => Promise<boolean>;
    getItemQuantity: (product: Product) => number;
    orderTotalPrice: number;
    //orderTotalCount: number;

    toOrder: (productId: string, inOrder: boolean) => void;

    orderAll: boolean;
    //decrease: (productId: string) => void;
    //increase: (productId: string) => void;
    clear: () => void;

    fetchItems: () => Promise<void>;

    //initialized: boolean;

    loading: boolean;
    error: boolean;
    errorText: string | null;
}

export const createCartStore = (
    auth: ReturnType<typeof createAuthStore>,
    //catalog: ReturnType<typeof createProductsStore>
): CartStore => {
    const {cartApiUrl, catalogApiUrl} = shopConfig;

    const cartApi = new CartApi(cartApiUrl, auth);

    const store = {
        items: new Map<string, CartItem>(),

        //initialized: false,

        get orderAll(): boolean {
            let c = true;
            this.items.forEach((v) => (c &&= v.toOrder));
            return c;
        },

        get count(): number {
            let c = 0;
            this.items.forEach((v) => (c += v.qty ? v.qty : 0));
            return c;
        },

        get orderTotalPrice(): number {
            let c = 0;
            this.items.forEach((v) => (c += v.toOrder ? v.product.price * v.qty : 0));
            return c;
        },

        get total(): number {
            let sum = 0;
            this.items.forEach((v) => (sum += v.product.price * v.qty));
            return sum;
        },

        async fetchItems(): Promise<void> {
            runInAction(() => {
                this.loading = true;
                this.error = false;
                this.errorText = "";
                this.items.clear();
            });

            try {
                const loadedItems = await cartApi.getItems();

                if (!loadedItems) {
                    runInAction(() => {
                        this.loading = false;
                        this.error = true;
                        this.errorText = "Не удалось загрузить корзину";
                    });
                    return;
                }

                for (const item of loadedItems.items) {
                    try {
                        const response = await fetch(`${catalogApiUrl}products/${item.productId}`);
                        const dto = await response.json() as ProductItemResponseDto;
                        const newItem = {
                            productId: item.productId,
                            product: {
                                id: dto.id,
                                article: dto.article,
                                title: dto.productTitle,
                                description: dto.productDescription,
                                price: dto.price,
                                imageUrl: dto.imageUrl,
                                category: dto.productCategory,
                                //TODO: stockQuantity: dto.stockQuantity
                            },
                            qty: item.quantity,
                            toOrder: item.toOrder
                        } as CartItem;
                        this.items.set(item.productId, newItem);
                    } catch (e) {
                        logger.error(`Failed to get product with id=${item.productId}`, e);
                    }
                }


            } catch (error: any) {
                logger.error("fetchItems error", error);
                runInAction(() => {
                    this.items.clear();
                    this.error = true;
                    this.errorText = error.message;
                });
            }
            finally {
                runInAction(() => {
                    this.loading = false;
                });
            }


            // await cartApi
            //     .getItems()
            //     .then(async (dto) => {
            //         runInAction(() => {
            //             dto.items.forEach(async (x: CartItemResponseDto) => {
            //
            //                     const response = await fetch(`${catalogApiUrl}products/${x.productId}`);
            //                     const product = await response.json() as Product;
            //
            //                     var newItem = {
            //                         toOrder: false,
            //                         product: product,
            //                         //     {
            //                         //     id: x.productId,
            //                         //     imageUrl: undefined,
            //                         //     title: "title",
            //                         //     price: 150,
            //                         //     article: 0,
            //                         //     description: "",
            //                         //     category: "",
            //                         // },
            //                         qty: x.quantity,
            //                     };
            //
            //                     console.error("new cart item: ", x);
            //                     this.items.set(x.productId, newItem);
            //                 }
            //             );
            //         });
            //     })
            //     .catch((reason) => {
            //         runInAction(() => {
            //             this.items.clear();
            //             this.error = true;
            //             this.errorText = reason.message;
            //         });
            //     })
            //     .finally(() => {
            //         runInAction(() => {
            //             this.loading = false;
            //         });
            //
            //         console.error(this.items)
            //     });
        },

        getItemQuantity(product: Product) {
            const qty = this.items.get(product.id)?.qty ?? 0;
            if (qty === undefined) {
                return 0;
            }

            return qty;
        },

        async add(product: Product): Promise<CartItem> {

            //console.log(this.items.get(product.id));

            const existingItem = this.items.get(product.id);
            console.warn("existing item", existingItem);

            if (existingItem) {
                const updatedItem = await cartApi.updateItem({...existingItem, qty: existingItem.qty + 1} as CartItem);
                if (updatedItem !== undefined) {
                    runInAction(() => {
                        //existingItem.qty = updatedItem.qty;
                        this.items.set(product.id, updatedItem as CartItem);
                    })
                    
                }
                return toJS(existingItem);

            } else {

                const newItem = {product: product, qty: 1, toOrder: true} as CartItem;
                const insertedItem = await cartApi.addItem(newItem);
                if (insertedItem) {
                    runInAction(() => {
                        this.items.set(product.id, insertedItem as CartItem);
                    });
                }

                return toJS(newItem);
            }
            //return toJS(existingItem);
        },

        // async addWithQuantity(product: Product, quantity: number) {
        //     if (quantity < 1) return;
        //
        //     let rec = this.items.get(product.id);
        //
        //     if (rec) {
        //         rec.qty += quantity;
        //
        //         await cartApi
        //             .updateItem(rec)
        //             .then((x) => {
        //                 logger.debug(`cart item updated: ${JSON.stringify(x)}`);
        //                 return rec;
        //             })
        //             .catch((reason) => {
        //                 logger.error("update error:", reason);
        //             });
        //     } else {
        //         rec = {product, qty: quantity, toOrder: true};
        //         // await cartApi
        //         //     .addItem(rec)
        //         //     .then((dto) => {
        //         //         runInAction(() => {
        //         //             this.items.set(product.id, dto);
        //         //         });
        //         //         return rec;
        //         //     })
        //         //     .catch((reason) => {
        //         //         console.log(reason);
        //         //     });
        //     }
        //     return toJS(rec);
        // },

        async remove(productId: string, all: boolean): Promise<boolean> {
            try {
                const existingItem = this.items.get(productId);

                if (existingItem) {
                    console.warn("existing item qty", existingItem.qty);
                    if(existingItem.qty === 1 || all) {
                        await cartApi.deleteItem(existingItem)
                        runInAction(() => {
                            this.items.delete(productId);                            
                        })                        
                    }
                    else {
                        const updatedItem = await cartApi.updateItem({...existingItem, qty: existingItem.qty - 1} as CartItem);
                        if (updatedItem !== undefined) {
                            runInAction(() => {
                                //existingItem.qty = updatedItem.qty;
                                this.items.set(productId, updatedItem as CartItem);
                            })

                        }
                    }
                    return true;
                }
                return false;

            } catch (e) {
                console.error(e);
                return false;
            }
        },

        find(productId: string): CartItem | undefined {
            return this.items.get(productId);
        },

        // decrease(productId: string): void {
        //     const rec = this.items.get(productId);
        //     if (!rec) return;
        //    
        //     rec.qty -= 1;
        //     if (rec.qty <= 0) this.items.delete(productId);
        // },
        //
        // increase(productId: string): void {
        //     const rec = this.items.get(productId);
        //     if (!rec) return;
        //     rec.qty += 1;
        //     if (rec.qty <= 0) this.items.delete(productId);
        // },

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
        errorText: "",
    };

    return makeAutoObservable(store);
};

export default createCartStore;
