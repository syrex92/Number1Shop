import {observer} from "mobx-react-lite";
import {useStores} from "../../context/RootStoreContext.tsx";
import {
    Grid,
    Group,
    Card, Button, Text, NumberFormatter
} from "@mantine/core";

import {
    IconArrowsRight,
} from "@tabler/icons-react";

const CartItemsFooter = observer(() => {

    const {cart} = useStores();

    return (
        <>
            <Card shadow="xs" p="md" radius={"sm"}>
                <Grid>
                    <Grid.Col span={6}>
                        <Group justify={"flex-start"} align={"center"}>
                            <Text fw={700} size={"xl"}>Итого: </Text>

                            <Text fw={700} size={"xl"}>
                            <NumberFormatter                                
                                thousandSeparator=" "
                                decimalSeparator=","
                                value={cart.orderTotalPrice}
                                suffix={" ₽"}
                                decimalScale={2}
                            />
                            </Text>

                        </Group>
                    </Grid.Col>

                    <Grid.Col span={6}>
                        <Group justify={"right"}>
                            <Button rightSection={<IconArrowsRight size={14} style={{width: '80%', height: '80%'}}/>}
                                disabled={cart.orderTotalPrice === 0}>
                                Заказать
                            </Button>
                        </Group>
                    </Grid.Col>
                </Grid>
            </Card>
        </>
    )
})

export default CartItemsFooter;