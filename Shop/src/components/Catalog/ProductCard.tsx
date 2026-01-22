import { observer } from "mobx-react-lite";
import { useStores } from "../../context/RootStoreContext.tsx";
import { FiHeart } from "react-icons/fi";
import type { Product } from "../../stores/ProductsStore.tsx";
import "../../styles/ProductCard.css";
import AddToCartButton from "../Cart/AddToCartButton.tsx";
import { Button, CloseIcon, Flex, Group, Stack } from "@mantine/core";
import { IconEdit } from "@tabler/icons-react";
import shopConfig from "../../config/shopConfig.ts";

interface ProductCardProps {
  product: Product;
  isFavorite: boolean;
  onDelete(productId: string): void;
  onEdit(productId: string): void;
  onViewCard(product: Product): void;
}

const ProductCard = observer(
  ({ product, isFavorite, onDelete, onEdit, onViewCard }: ProductCardProps) => {
    const { favorites } = useStores();
    const {catalogApiUrl} = shopConfig;

    const handleViewClick = () => {
      onViewCard(product);
    };

    return (
      <>
        <div className="product-card card">
          <div className="image-wrap">
            <img src={`${catalogApiUrl}/${product.imageUrl}`} alt={product.title} className="product-image" />
            <Flex
              mih={50}
              bg="rgba(0, 0, 0, .3)"
              gap="md"
              justify="flex-end"
              align="flex-start"
              direction="row"
              wrap="wrap"
            >
              <button
                className={`favorite ${isFavorite ? "active" : ""}`}
                onClick={() => favorites.toggle(product.id)}
                aria-label="Добавить в избранное"
                title={isFavorite ? "Убрать из избранного" : "В избранное"}
              >
                <FiHeart />
              </button>
              <button
                className="edit"
                onClick={() => onEdit(product.id)}
                aria-label="Редактировать товар"
                title="Редактировать товар"
              >
                <IconEdit />
              </button>
              <button
                className="close"
                onClick={() => onDelete(product.id)}
                aria-label="Удалить товар"
                title="Удалить товар"
              >
                <CloseIcon />
              </button>
            </Flex>
          </div>

          <Stack justify="flex-end" align="stretch">
            <div className="product-title">{product.title}</div>
            <div className="product-price">{product.price} ₽</div>

            <Group justify="center" gap="sm">
              <AddToCartButton product={product} />
              <Button onClick={handleViewClick}>Просмотр товара</Button>
            </Group>
          </Stack>
        </div>
      </>
    );
  },
);

export default ProductCard;
