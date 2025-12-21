import { Button, Card, Image, Text, Group } from "@mantine/core";
import type { Product } from "../../../stores/ProductsStore";
import { useState } from "react";

interface IViewProductProps {
  product: Product;
  onConfirm(product: Product, productCount: number): void;
  onCancel(): void;
}

const ViewProductComponent = ({ product, onConfirm, onCancel }: IViewProductProps) => {
  const [productCount, setProductCount] = useState<number>(0);
  const handleClick = () => {
    setProductCount((prevCount) => prevCount + 1);
  };

  const handleDeleteClick = () => {
    setProductCount((prevCount) => prevCount - 1);
  };

  const handleCompleteClick = () => {
    onConfirm(product, productCount);
  };

  return (
    <>
      <Card shadow="sm" padding="lg" radius="md" withBorder>
        <Card.Section>
          <Image src={product.image} height={200} alt="ProductImage" />
        </Card.Section>

        <Group justify="space-between" mt="md" mb="xs">
          <Text fw={500}>{product.title}</Text>
        </Group>

        <Text size="sm" c="dimmed">
          {product.description}
        </Text>

        <Button
          color="blue"
          fullWidth
          mt="md"
          radius="md"
          onClick={handleClick}
        >
          {productCount == 0 ? "Добавить в корзину" : productCount}
        </Button>

        {productCount !== 0 && (
          <Button
            color="blue"
            fullWidth
            mt="md"
            radius="md"
            onClick={handleDeleteClick}
          >
            Убрать из корзины
          </Button>
        )}

        <Button
          color="blue"
          fullWidth
          mt="md"
          radius="md"
          onClick={handleCompleteClick}
        >
          Подтвердить
        </Button>

        <Button
          color="blue"
          fullWidth
          mt="md"
          radius="md"
          onClick={onCancel}
        >
          Отменить
        </Button>
      </Card>
    </>
  );
};

export default ViewProductComponent;
