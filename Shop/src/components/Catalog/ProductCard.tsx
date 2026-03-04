import { observer } from "mobx-react-lite";
import { useStores } from "../../context/RootStoreContext.tsx";
import type { Product } from "../../stores/ProductsStore.tsx";
import "../../styles/ProductCard.css";
import AddToCartButton from "../Cart/AddToCartButton.tsx";
import {
  Button,
  Card,
  Group,
  Image,
  Text,
  Box,
  Badge,
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
    const { siteUrl } = shopConfig;

    const handleViewClick = () => {
      onViewCard(product);
    };

    return (
      <Card shadow="sm" radius="md" withBorder h="100%">
        <Group gap="sm">
          <Box pos="relative" component="div">
            <Image
              src={`${siteUrl}images/${product.imageUrl}`}
              height={160}
              fit="contain"
              fallbackSrc="/public/logo192.png"
            />

            <Group gap={6} pos="absolute" top={8} right={8}>
              {auth.user?.role === 'user' && (
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
              {auth.user?.role === 'admin' && (
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
          <Text fw={500}>{product.title}</Text>
        </Group>
        <Group
          justify="center"
          gap="sm"
          style={{
            display: "flex",
            flexDirection: "column",
            marginTop: "auto",
          }}
        >
          <Badge
            size={"xl"}
            p={10}
            color={"orange"}
            m={5}
            variant="transparent"
            radius="xs"
          >
            {product.price} ₽
          </Badge>
          {auth.user?.role === 'user' && <AddToCartButton product={product} />}
          <Button size="sm" variant="light" onClick={handleViewClick}>
            Просмотр товара
          </Button>
        </Group>
      </Card>
    );
  },
);

export default ProductCard;
