import {observer} from "mobx-react-lite";
import {useStores} from "../../context/RootStoreContext.tsx";
import {
    Grid,
    Card, Checkbox
} from "@mantine/core";


const CartItemsHeader = observer(() => {

    const {cart} = useStores();
    
    return (
        <>
            <Card shadow="xs" p="md" radius={"sm"}>
                <Grid>
                    <Grid.Col span={4}>
                        <Checkbox
                            checked={cart.orderAll}    
                            label={"Выбрать все"} onChange={(event) => {
                            cart.items.forEach((element) => { 
                                cart.toOrder(element.product.id, event.target.checked) 
                            });
                        }} />
                    </Grid.Col>

                    <Grid.Col span={8}>
                        
                    </Grid.Col>
                </Grid>
            </Card>
        </>
    )
})

export default CartItemsHeader;