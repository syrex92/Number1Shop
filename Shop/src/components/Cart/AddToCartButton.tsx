import {ActionIcon, Button, Group, Text} from "@mantine/core";
import {IconMinus, IconPlus, IconX} from "@tabler/icons-react";
import type {Product} from "../../stores/ProductsStore.tsx";
import {useStores} from "../../context/RootStoreContext.tsx";
import {observer} from "mobx-react-lite";
import {BeatLoader} from "react-spinners";
import {useState} from "react";
import {openConfirmModal} from "@mantine/modals";

interface AddToCartButtonProps {
    product: Product;
}

const AddToCartButton = observer(({product}: AddToCartButtonProps) => {
    const {cart} = useStores();

    const quantity = cart.getItemQuantity(product);

    const [exec, setExec] = useState<boolean>(false);

    function openDeleteDialog() {
        console.log("openDeleteDialog");
        openConfirmModal({
            title: 'Подтвердите удаление',
            children: (
                <Text size="sm">
                    Удалить товар из корзины?
                </Text>
            ),
            labels: {confirm: 'Удалить', cancel: 'Отмена'},
            confirmProps: {color: 'red'},
            onConfirm: async () => {
                setExec(true)
                try {
                    await cart.remove(product.id, true)
                } finally {
                    setExec(false)
                }
            },
            onCancel: () => {
            },
        });
    }

    return (
        <>
            {quantity === 0 ? (
                <Button size="sm"
                        variant="light"
                        onClick={async () => {
                            await cart.add(product);
                        }}
                        loading={cart.loading}>
                    В корзину
                </Button>
            ) : (
                <Group gap={4} h={30}>

                    {exec && <BeatLoader color={"green"} size={5}/>}

                    {!exec && (
                        <>
                            <ActionIcon
                                size="sm"
                                variant="light"
                                onClick={async () => {
                                    setExec(true);
                                    try {
                                        await cart.remove(product.id, false)
                                    } finally {
                                        setExec(false);
                                    }

                                }}
                            >
                                <IconMinus size={14}/>
                            </ActionIcon>

                            <Text fw={500} w={24} ta="center">
                                {quantity}
                            </Text>

                            <ActionIcon
                                size="sm"
                                variant="light"
                                onClick={async () => {
                                    setExec(true);
                                    try {
                                        await cart.add(product)
                                    } finally {
                                        setExec(false);
                                    }

                                }}
                            >
                                <IconPlus size={14}/>
                            </ActionIcon>

                            <ActionIcon
                                size="sm"
                                variant="subtle"
                                color="red"
                                onClick={async () => {
                                    setExec(true);
                                    try {
                                        openDeleteDialog()
                                    } finally {
                                        setExec(false);
                                    }
                                }}
                            >
                                <IconX size={14}/>
                            </ActionIcon>
                        </>
                    )}
                </Group>
            )}
        </>
    );
});

export default AddToCartButton;
