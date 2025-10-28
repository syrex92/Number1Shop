import {observer} from "mobx-react-lite";
import {useStores} from "../../context/RootStoreContext.tsx";
import type {CartItem} from "../../stores/CartStore.tsx";
import {
    Checkbox,
    Grid,
    Image,
    Text,
    Flex,
    Group,
    ActionIcon,
    Card,
    NumberFormatter
} from "@mantine/core";
import {useEffect} from "react";
import {
    IconHeart,
    IconMinus,
    IconPlus,
    IconTrash
} from "@tabler/icons-react";

export interface CartItemCardProps {
    cartItem: CartItem;
}


const CartItemCard = observer(({cartItem}: CartItemCardProps) => {

    const {cart} = useStores();
    //const navigate = useNavigate();

    useEffect(() => {
        console.log(cartItem)
    }, [])

    return (
        <>
            <Card shadow="xs" p="md" radius={"sm"}>
                <Grid>
                    <Grid.Col span={1}>
                        <Checkbox/>
                    </Grid.Col>
                    <Grid.Col span={2}>
                        <Image radius={10} src={cartItem.product.image}/>
                    </Grid.Col>
                    <Grid.Col span={6}>
                        <Flex justify={"center"} align={"flex-start"} direction={"column"}>
                            <Text size={"xl"}>{cartItem.product.title}</Text>
                            <Text size={"md"}>Описание товара</Text>
                            <Group m={"xs"} align={"flex-start"}>
                                <ActionIcon variant="outline" aria-label="Settings"
                                            onClick={() => console.log("Set favourite")}>
                                    <IconHeart style={{width: '80%', height: '80%'}} stroke={1.5}/>
                                </ActionIcon>

                                <ActionIcon variant="outline" aria-label="Settings"
                                            onClick={() => cart.remove(cartItem.product.id)}>
                                    <IconTrash style={{width: '80%', height: '80%'}} stroke={1.5}/>
                                </ActionIcon>
                            </Group>
                        </Flex>
                    </Grid.Col>
                    <Grid.Col span={3}>
                        <Group justify={"center"}>

                            <ActionIcon variant="outline" aria-label="Settings"
                                        onClick={() => cart.decrease(cartItem.product.id)}>
                                <IconMinus style={{width: '80%', height: '80%'}} stroke={1.5}/>
                            </ActionIcon>

                            <Text>{cartItem.qty}</Text>

                            <ActionIcon variant="outline" aria-label="Settings"
                                        onClick={() => cart.add(cartItem.product)}>
                                <IconPlus style={{width: '80%', height: '80%'}} stroke={1.5}/>
                            </ActionIcon>

                            <NumberFormatter
                                thousandSeparator=" "
                                decimalSeparator=","
                                value={cartItem.qty * cartItem.product.price}
                                suffix={" руб."}
                                decimalScale={2}
                            />

                        </Group>
                    </Grid.Col>
                </Grid>
            </Card>            
        </>
    )
})

export default CartItemCard;