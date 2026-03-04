import { makeAutoObservable, runInAction } from "mobx";
import shopConfig from "../config/shopConfig.ts";
import type createAuthStore from './AuthStore';

export interface NewProduct {
  title: string;
  price: number;
  article: number;
  category: string;
  description: string | undefined;
  image: File | null;
}
export interface Product {
  id: string;
  title: string;
  price: number;
  article: number;
  description: string | undefined;
  category: string;
  imageUrl?: string;
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
  updateProductImage: (productId: string, file: File) => Promise<void>;
}

export interface ProductItemResponseDto {
  id: string;
  stockQuantity: number;
  productTitle: string;
  productDescription: string;
  productCategory: string;
  article: number;
  price: number;
  imageUrl?: string;
}

export interface CreateProductRequest {
  productTitle: string;
  productDescription: string | undefined;
  productCategory: string;
  article: number;
  price: number;
  image: File | null;
}

export interface UpdateProductRequest {
  productTitle: string | undefined;
  productDescription: string | undefined;
  productCategory: string | undefined;
  article: number | undefined;
  price: number | undefined;
}

export const createProductsStore = (_auth: ReturnType<typeof createAuthStore>): ProductsStore => {
  const { catalogApiUrl } = shopConfig;

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

      fetch(`${catalogApiUrl}products/`)
        .then((res) => res.json())
        .then(async (res) => {
          const loadedItems = res as ProductItemResponseDto[];

          console.log(loadedItems);

          const products = new Array<Product>();
          loadedItems.forEach((x: ProductItemResponseDto) =>
            products.push({
              id: x.id,
              title: x.productTitle,
              price: x.price,
              article: x.article,
              description: x.productDescription,
              category: x.productCategory,
              imageUrl: x.imageUrl,
            }),
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

      const formData = new FormData();

      formData.append("productTitle", data.title);
      formData.append("price", String(data.price));
      formData.append("article", String(data.article));
      formData.append("productCategory", data.category);

      if (data.description) {
        formData.append("productDescription", data.description);
      }

      if (data.image) {
        formData.append("image", data.image);
      }

      fetch(`${catalogApiUrl}/products/`, {
        method: "POST",
        body: formData,
        headers: _auth.getAuthHeaders()
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
                imageUrl: createdItem.imageUrl,
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
        productTitle: data.title,
      };

      fetch(`${catalogApiUrl}products/${data.id}`, {
        method: "PUT",
        headers: _auth.getAuthHeaders(),
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
                    imageUrl: updatedItem.imageUrl,
                    price: updatedItem.price,
                  }
                : item,
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

    async updateProductImage(productId: string, file: File): Promise<void> {
      const formData = new FormData();
      formData.append("file", file);

      await fetch(`${catalogApiUrl}products/${productId}/image`, {
        method: "POST",
        body: formData,
        headers: _auth.getAuthHeaders(),
      });

      console.log("Product image updated");
      await this.fetchProducts();
    },

    async deleteProduct(productId: string): Promise<void> {
      runInAction(() => {
        this.isLoading = true;
      });

      console.log("Start delete product");

      await fetch(`${catalogApiUrl}products/${productId}`, {
        method: "DELETE",
        headers: _auth.getAuthHeaders()
      })
        .then(() =>
          runInAction(() => {
            this.products = this.products.filter(
              (item) => item.id !== productId,
            );
            console.log("product deleted");
          }),
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
