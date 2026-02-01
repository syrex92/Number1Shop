import { observer } from "mobx-react-lite";
import { useState } from "react";
import {
  Card,
  Image,
  Text,
  Group,
  Stack,
  Button,
  NumberInput,
} from "@mantine/core";
import { useStores } from "../../../context/RootStoreContext";
import type { Product } from "../../../stores/ProductsStore";
import shopConfig from "../../../config/shopConfig";

interface Props {
  product: Product;
  onClose(): void;
}

const ViewProductComponent = observer(({ product, onClose }: Props) => {
  const { cart, auth } = useStores();
  const [quantity, setQuantity] = useState(0);
  const { catalogApiUrl } = shopConfig;

  const handleAddClick = () => {
    setQuantity(1);
  };

  const handleConfirm = () => {
    cart.addWithQuantity(product, quantity);
    onClose();
  };

  const handleCancel = () => {
    setQuantity(0);
    onClose();
  };

  return (
    <Card radius="md" withBorder>
      <Stack gap="md">
        <Image
          src={`${catalogApiUrl}/${product.imageUrl}`}
          height={240}
          fit="contain"
          fallbackSrc="/no-image.png"
        />

        <Text fw={600} size="lg">
          {product.title}
        </Text>

        <Text size="sm" c="dimmed">
          {product.description}
        </Text>

        {auth.user?.role == "user" && (
          <>
            {quantity === 0 ? (
              <Button size="sm" variant="light" onClick={handleAddClick}>
                Добавить в корзину
              </Button>
            ) : (
              <Stack gap="sm">
                <NumberInput
                  label="Количество"
                  min={1}
                  value={quantity}
                  onChange={(value) => setQuantity(Number(value))}
                />

                <Group justify="space-between">
                  <Button
                    size="sm"
                    variant="light"
                    color="gray"
                    onClick={handleCancel}
                  >
                    Отменить
                  </Button>

                  <Button size="sm" variant="filled" onClick={handleConfirm}>
                    Подтвердить
                  </Button>
                </Group>
              </Stack>
            )}
          </>
        )}
      </Stack>
    </Card>
  );
});

export default ViewProductComponent;
