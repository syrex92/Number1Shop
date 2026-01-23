import { observer } from "mobx-react-lite";
import { useStores } from "../../context/RootStoreContext.tsx";
import type { Product } from "../../stores/ProductsStore.tsx";
import "../../styles/ProductCard.css";
import AddToCartButton from "../Cart/AddToCartButton.tsx";
import {
  Button,
  Card,
  Group,
  Stack,
  Image,
  Text,
  Box,
  ActionIcon,
} from "@mantine/core";
import { IconEdit, IconHeart, IconTrash } from "@tabler/icons-react";
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
    const { favorites, auth } = useStores();
    const { catalogApiUrl } = shopConfig;

    const handleViewClick = () => {
      onViewCard(product);
    };

    return (
      <>
        <Card shadow="sm" radius="md" withBorder h="100%">
          <Stack gap="sm">
            {/* IMAGE + ACTIONS */}
            <Box pos="relative">
              <Image
                src={`${catalogApiUrl}/${product.imageUrl}`}
                height={160}
                fit="contain"
                fallbackSrc="/public/logo192.png"
              />

              <Group gap={6} pos="absolute" top={8} right={8}>
                {/* Должен быть зареганный */}
                {auth.role == "guest" && (
                  <ActionIcon
                    size="md"
                    radius="md"
                    variant={isFavorite ? "filled" : "light"}
                    color={isFavorite ? "red" : "gray"}
                    onClick={() => favorites.toggle(product.id)}
                  >
                    <IconHeart size={16} />
                  </ActionIcon>
                )}
                {/* Должен быть админ */}
                {auth.role == "guest" && (
                  <>
                    <ActionIcon
                      size="md"
                      radius="md"
                      variant="light"
                      onClick={() => onEdit(product.id)}
                    >
                      <IconEdit size={16} />
                    </ActionIcon>

                    <ActionIcon
                      size="md"
                      radius="md"
                      variant="light"
                      color="red"
                      onClick={() => onDelete(product.id)}
                    >
                      <IconTrash size={16} />
                    </ActionIcon>
                  </>
                )}
              </Group>
            </Box>
            {/* CONTENT */}
            <Text fw={500}>{product.title}</Text>

            <Text size="sm" c="dimmed" lineClamp={2}>
              {product.description}
            </Text>

            <Text fw={700}>{product.price} ₽</Text>

            <Group justify="center" gap="sm">
              {/* Должен быть зареганный пользователь */}
              {auth.role == "guest" && <AddToCartButton product={product} />}
              <Button size="sm" variant="light" onClick={handleViewClick}>
                Просмотр товара
              </Button>
            </Group>
          </Stack>
        </Card>
      </>
    );
  },
);

export default ProductCard;
