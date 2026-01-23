import { ActionIcon, Button, Group, Text } from "@mantine/core";
import { IconMinus, IconPlus, IconX } from "@tabler/icons-react";
import type { Product } from "../../stores/ProductsStore.tsx";
import { useStores } from "../../context/RootStoreContext.tsx";
import { observer } from "mobx-react-lite";

interface AddToCartButtonProps {
  product: Product;
}

const AddToCartButton = observer(({ product }: AddToCartButtonProps) => {
  const { cart } = useStores();

  const quantity = cart.getItemQuantity(product);

  return (
    <>
      {quantity === 0 ? (
        <Button size="sm" variant="light" onClick={() => cart.add(product)} loading={cart.loading}>
          В корзину
        </Button>
      ) : (
        <Group gap={4}>
          <ActionIcon
            size="sm"
            variant="light"
            onClick={() => cart.decrease(product.id)}
          >
            <IconMinus size={14} />
          </ActionIcon>

          <Text fw={500} w={24} ta="center">
            {quantity}
          </Text>

          <ActionIcon
            size="sm"
            variant="light"
            onClick={() => cart.add(product)}
          >
            <IconPlus size={14} />
          </ActionIcon>

          <ActionIcon
            size="sm"
            variant="subtle"
            color="red"
            onClick={() => cart.decrease(product.id)}
          >
            <IconX size={14} />
          </ActionIcon>
        </Group>
      )}
    </>
  );
});

export default AddToCartButton;
