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

const ProductsPage = observer(() => {
  const { products, favorites } = useStores();
  const [isNewModalOpen, setIsNewModalOpen] = useState<boolean>(false);
  const [productForEdit, setProductForEdit] = useState<Product | undefined>(
    undefined
  );

  useEffect(() => {
    products.fetchProducts();
  }, []);

  const handleDelete = (productId: string) => {
    products.deleteProduct(productId);
  };

  const handleConfirm = (product: Product | NewProduct, isCreate: boolean) => {
    if (isCreate) {
      products.createProduct(product as NewProduct);
      return;
    }

    products.updateProduct(product as Product);
  };

  const handleEdit = (productId: string) => {
    setProductForEdit(
      products.filteredProducts.find((p) => p.id === productId)
    );
  };

  useEffect(() => {
    if (productForEdit) {
      setIsNewModalOpen(true);
    }
  }, [productForEdit]);

  const handleClick = (product: Product) => {
    // Добавить просмотр товара
    console.log(product);
  };

  return (
    <>
      <Modal
        title={productForEdit ? "Обновить товар" : "Новый товар"}
        opened={isNewModalOpen}
        onClose={() => setIsNewModalOpen(false)}
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
              onClick={handleClick}
            />
          ))}
        </div>
      )}
    </>
  );
});

export default ProductsPage;
