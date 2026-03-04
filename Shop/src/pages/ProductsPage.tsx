import { observer } from "mobx-react-lite";
import { useStores } from "../context/RootStoreContext";
import ProductCard from "../components/Catalog/ProductCard";
import "../styles/ProductsPage.css";
import { useEffect, useState } from "react";
import {
  Button,
  Center,
  Group,
  Loader,
  Modal,
  Stack,
  Text,
} from "@mantine/core";
import ProductComponent from "../components/Catalog/ProductComponent/ProductComponent";
import type { NewProduct, Product } from "../stores/ProductsStore";
import ViewProductComponent from "../components/Catalog/ProductComponent/ViewProductComponent";

const ProductsPage = observer(() => {
  const { products, favorites, auth } = useStores();
  const [isNewModalOpen, setIsNewModalOpen] = useState<boolean>(false);
  const [isViewModalOpen, setIsViewModalOpen] = useState<boolean>(false);
  const [productForEdit, setProductForEdit] = useState<Product | undefined>(
    undefined,
  );
  const [productForView, setProductForView] = useState<Product | undefined>(
    undefined,
  );

  useEffect(() => {
    products.fetchProducts();
  }, []);

  const handleDelete = (productId: string) => {
    products.deleteProduct(productId);
  };

  const handleConfirm = (
    product: Product | NewProduct,
    isCreate: boolean,
    file: File | null,
  ) => {
    if (isCreate) {
      products.createProduct(product as NewProduct);
      return;
    }

    const prod = product as Product;

    if (file && prod.imageUrl !== file.name) {
      products.updateProductImage(prod.id, file);
    }
    products.updateProduct(prod);
  };

  const handleEdit = (productId: string) => {
    setProductForEdit(
      products.filteredProducts.find((p) => p.id === productId),
    );
  };

  useEffect(() => {
    if (productForEdit) {
      setIsNewModalOpen(true);
    }
  }, [productForEdit]);

  useEffect(() => {
    if (productForView) {
      setIsViewModalOpen(true);
    }
  }, [productForView]);

  const handleClick = (product: Product) => {
    setProductForView(product);
  };

  return (
    <>
      <Modal
        title={productForEdit ? "Обновить товар" : "Новый товар"}
        opened={isNewModalOpen}
        onClose={() => {
          if (productForEdit) {
            setProductForEdit(undefined);
          }
          setIsNewModalOpen(false);
        }}
      >
        <ProductComponent
          product={productForEdit}
          onCancel={() => {
            if (productForEdit) {
              setProductForEdit(undefined);
            }
            setIsNewModalOpen(false);
          }}
          onConfirm={handleConfirm}
        />
      </Modal>

      <Modal
        title={"Просмотр товара"}
        opened={isViewModalOpen}
        onClose={() => {
          if (productForView) {
            setProductForView(undefined);
          }
          setIsViewModalOpen(false);
        }}
      >
        {productForView && (
          <ViewProductComponent
            product={productForView}
            onClose={() => {
              setIsViewModalOpen(false);
              setProductForView(undefined);
            }}
          />
        )}
      </Modal>

      {auth.user && auth.user.role === 'admin' && (
        <div>
          <Group mb="10">
            <Button
              variant="filled"
              color="orange"
              onClick={() => setIsNewModalOpen(true)}
            >
              Добавить товар
            </Button>
          </Group>
        </div>
      )}
      
      {products.isLoading ? (
        <Center>
          <Stack justify="center" align="center">
            <Loader size={64} />
            <Text>Загрузка каталога...</Text>
          </Stack>
        </Center>
      ) : (
        <div className="products-grid">
          {products.filteredProducts.map((p) => (
            <ProductCard
              key={p.id}
              product={p}
              isFavorite={favorites.isFavorite(p.id)}
              onDelete={handleDelete}
              onEdit={handleEdit}
              onViewCard={handleClick}
            />
          ))}
        </div>
      )}
    </>
  );
});

export default ProductsPage;
