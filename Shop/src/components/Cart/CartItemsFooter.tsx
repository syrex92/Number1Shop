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
import {notifications} from "@mantine/notifications";
import {useState} from "react";
import {logger} from "../../utils/logger.ts";

const CartItemsFooter = observer(() => {

    const {cart, auth} = useStores();

    const {ordersApiUrl} = shopConfig;

    const ordersApi = new ApiClient(ordersApiUrl, auth);

    // function takeFirstLines(text: string, n: number): string {
    //     const lines = text.split(/\r?\n/);            // учтёт \n и \r\n
    //     return lines.slice(0, n).join('\n');
    // }


    const [creatingOrder, setCreatingOrder] = useState<boolean>(false);
    const createOrder = async () : Promise<boolean> => {
        let created = false;
        
        try {
            setCreatingOrder(true);


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
                createdAt: (new Date()).toISOString(),
                items: orderItems,
                status: "New",
                id: crypto.randomUUID(),
                userId: crypto.randomUUID(),
                deliveryAddress: address
            } as IOrderDto;

            const response = await ordersApi.post("", order);
            
            created = response.ok;
        }
        catch(error: any){
            logger.error(error);
        }
        finally {
            setCreatingOrder(false);
            if(created){
                notifications.show({
                    position: "top-center",
                    color: "green",
                    title: "Успех",
                    message: 'Заказ успешно оформлен и скоро появится в разделе "Заказы"'
                })  
            }
            else{
                notifications.show({
                    position: "top-center",
                    color: "red",
                    title: "Ошибка",
                    message: "Произошла ошибка при оформлении заказа. Попробуйте повторить позже"
                })
            }
        }
        
        return created;
        
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
                                    loading={creatingOrder}
                                    onClick={createOrder}
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