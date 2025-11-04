import {Button, Group, Indicator} from "@mantine/core";
import {IconShoppingCart} from "@tabler/icons-react";
import type {Product} from "../../stores/ProductsStore.tsx";
import {useStores} from "../../context/RootStoreContext.tsx";
import {useEffect, useState} from "react";
import type {CartItem} from "../../stores/CartStore.tsx";
import {observer} from "mobx-react-lite";

interface AddToCartButtonProps {
    product: Product;    
}

const AddToCartButton = observer(({product}: AddToCartButtonProps) => {

    const [cartItem, setCartItem] = useState<CartItem | undefined>(undefined);
    
    const {cart} = useStores();
    
    useEffect(() => {
        let itm = cart.find(product.id);
        console.log(itm);        
        setCartItem(itm);        
    }, [product])
    
    const onAddClick = () => {
        setCartItem(cart.add(product));
    }

    const inCart = cart.find(product.id) !== undefined;
    
    return (
        <div>
            <Group justify="center">
                <Indicator size="16" color="orange" label={cartItem?.qty} disabled={!inCart}>
                    <Button
                        onClick={onAddClick}
                        leftSection={<IconShoppingCart size={20} stroke={1.5}/>}
                        variant="default">
                        В корзину
                    </Button>
                </Indicator>
            </Group>        
            
        </div>


    )
})

export default AddToCartButton;