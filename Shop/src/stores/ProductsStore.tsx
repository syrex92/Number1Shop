import { makeAutoObservable, runInAction } from "mobx";
import shopConfig from "../config/shopConfig.ts";

export interface NewProduct {
  title: string;
  price: number;
  article: number;
  category: string;
  description: string | undefined;
  image: string | undefined;
}
export interface Product {
  id: string;
  title: string;
  price: number;
  article: number;
  description: string | undefined;
  category: string;
  image: string | undefined;
}

export interface ProductsStore {
  products: Product[];
  isLoading: boolean;
  query: string;
  filteredProducts: Product[];
  setQuery: (value: string) => void;
  fetchProducts: () => Promise<void>;
  createProduct: (data: NewProduct) => Promise<void>;
  updateProduct: (data: Product) => Promise<void>;
  deleteProduct: (productId: string) => Promise<void>;
}

export interface ProductItemResponseDto {
  id: string;
  stockQuantity: number;
  productTitle: string;
  productDescription: string;
  productCategory: string;
  article: number;
  price: number;
  imagesUrls: string[];
}

export interface CreateProductRequest {
  productTitle: string;
  productDescription: string | undefined;
  productCategory: string;
  article: number;
  price: number;
  imagesUrls: string[] | undefined;
}

export interface UpdateProductRequest {
  productTitle: string | undefined;
  productDescription: string | undefined;
  productCategory: string | undefined;
  article: number | undefined;
  price: number | undefined;
  imagesUrls: string[] | undefined;
}

export const createProductsStore = (): ProductsStore => {
    
    const {catalogApiUrl} = shopConfig
    
  const store = {
    products: [] as Product[],
    isLoading: false,
    query: "",

    get filteredProducts(): Product[] {
      const q = this.query.trim().toLowerCase();
      if (!q) return this.products;
      return this.products.filter((p) => p.title.toLowerCase().includes(q));
    },

    setQuery(value: string): void {
      this.query = value;
    },

    async fetchProducts(): Promise<void> {
      runInAction(() => {
        this.isLoading = true;
      });

      console.log("Start products fetching");

      fetch(catalogApiUrl)
        .then((res) => res.json())
        .then(async (res) => {
          const loadedItems = res as ProductItemResponseDto[];

          console.log(loadedItems);

          const products = new Array<Product>();
          loadedItems.forEach((x: ProductItemResponseDto) =>
            products.push({
              id: x.id,
              image:
                x.imagesUrls.length > 0
                  ? `/images/${x.imagesUrls[0]}`
                  : undefined,
              title: x.productTitle,
              price: x.price,
              article: x.article,
              description: x.productDescription,
              category: x.productCategory,
            })
          );

          runInAction(() => {
            this.products = products;
          });

          console.log("Products loaded");
        })
        .catch((reason) => {
          console.log("Products fetch error");
          console.log(reason);
        })
        .finally(() => {
          runInAction(() => {
            this.isLoading = false;
          });
        });
    },

    async createProduct(data: NewProduct): Promise<void> {
      runInAction(() => {
        this.isLoading = true;
      });

      console.log("Start create product");

      const request: CreateProductRequest = {
        article: data.article,
        price: data.price,
        productCategory: data.category,
        productDescription: data.description,
        imagesUrls: undefined,
        productTitle: data.title,
      };

      fetch(catalogApiUrl, {
        method: "POST",
        headers: {
          "Content-Type": "application/json", // Indicate that the body is JSON
        },
        body: JSON.stringify(request),
      })
        .then((res) => res.json())
        .then(async (res) => {
          const createdItem = res as ProductItemResponseDto;

          console.log(createdItem);

          runInAction(() => {
            this.products = [
              ...this.products,
              {
                id: createdItem.id,
                article: createdItem.article,
                category: createdItem.productCategory,
                description: createdItem.productDescription,
                price: createdItem.price,
                title: createdItem.productTitle,
                image: createdItem.imagesUrls[0],
              },
            ];
          });

          console.log("Product created");
        })
        .catch((reason) => {
          console.log("Product created error");
          console.log(reason);
        })
        .finally(() => {
          runInAction(() => {
            this.isLoading = false;
          });
        });
    },

    async updateProduct(data: Product): Promise<void> {
      runInAction(() => {
        this.isLoading = true;
      });

      console.log("Start update product");

      const request: UpdateProductRequest = {
        article: data.article,
        price: data.price,
        productCategory: data.category,
        productDescription: data.description,
        imagesUrls: undefined,
        productTitle: data.title,
      };

      fetch(`${catalogApiUrl}${data.id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json", // Indicate that the body is JSON
        },
        body: JSON.stringify(request),
      })
        .then((res) => res.json())
        .then(async (res) => {
          const updatedItem = res as ProductItemResponseDto;

          console.log(updatedItem);

          runInAction(() => {
            this.products = this.products.map((item) =>
              item.id === updatedItem.id
                ? {
                    ...item,
                    title: updatedItem.productTitle,
                    article: updatedItem.article,
                    category: updatedItem.productCategory,
                    description: updatedItem.productDescription,
                    image: updatedItem.imagesUrls[0],
                    price: updatedItem.price,
                  }
                : item
            );
          });

          console.log("Product updated");
        })
        .catch((reason) => {
          console.log("Product created error");
          console.log(reason);
        })
        .finally(() => {
          runInAction(() => {
            this.isLoading = false;
          });
        });
    },

    async deleteProduct(productId: string): Promise<void> {
      runInAction(() => {
        this.isLoading = true;
      });

      console.log("Start delete product");

      fetch(`${catalogApiUrl}${productId}`, {
        method: "DELETE",
        headers: {
          "Content-Type": "application/json", // Indicate that the body is JSON
        },
      })
        .then(() =>
          runInAction(() => {
            this.products = this.products.filter(
              (item) => item.id !== productId
            );
            console.log("product deleted")
          })
        )
        .catch((reason) => {
          console.log("Product delete error");
          console.log(reason);
        })
        .finally(() => {
          runInAction(() => {
            this.isLoading = false;
          });
        });
    },
  };

  return makeAutoObservable(store);
};

export default createProductsStore;
