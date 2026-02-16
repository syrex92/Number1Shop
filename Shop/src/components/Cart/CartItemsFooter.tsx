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
import type {IOrderItemDto} from "../../dto/IOrderItemDto.ts";
import type {IOrderDto} from "../../dto/IOrderDto.ts";
import type {IAddressDto} from "../../dto/IAddressDto.ts";
import shopConfig from "../../config/shopConfig.ts";
import ApiClient from "../../utils/api.ts";

const CartItemsFooter = observer(() => {

    const {cart, auth} = useStores();

    const {ordersApiUrl} = shopConfig;

    const ordersApi = new ApiClient(ordersApiUrl, auth);

    const createOrder = async () : Promise<boolean> => {
        const itemsToOrder = Array.from(cart.items.values())
            .filter((item) => item.toOrder)
        
        const orderItems = itemsToOrder.map((item) => {
            return {
                product: item.product.id,
                quantity: item.qty,
                cost: item.product.price
            } as IOrderItemDto;
        })
        
        const address = {
            country: "Россия",
            city: "Краснодар",
            street: "Красная",
            house: "19",
            appartment: "99",
            postalCode: "350011"            
        } as IAddressDto;
        
        const order = {
            createdAt: Date.now().toString(),
            items: orderItems,
            status: "New",
            id: crypto.randomUUID(),//"3fa85f64-5717-4562-b3fc-2c963f66afa6",
            userId: "",
            deliveryAddress: address
        } as IOrderDto;

        
        
        console.log("order to create: ", order);
        
        const response = await ordersApi.post("", order);
        
        console.log("response", response);
        
        return response.ok;
        
    }
    
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
                                disabled={cart.orderTotalPrice === 0}
                                    onClick={() => createOrder()}
                            >
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