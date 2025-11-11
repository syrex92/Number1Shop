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
    const [executing, setExecuting] = useState<boolean>(false);
    
    const {cart} = useStores();
    
    useEffect(() => {
        
        
        const cartItem = cart.find(product.id);
        if(cartItem)
            setCartItem(cartItem);        
    }, [product])
    
    const onAddClick = () => {
        setExecuting(true);
        cart.add(product).then((itm) => {
            setCartItem(itm);
            setExecuting(false);            
        })        
    }

    const inCart = cart.find(product.id) !== undefined;
    
    return (
        <div>
            <Group justify="center">
                <Indicator size="16" color="orange" label={cartItem?.qty} disabled={!inCart}>
                    <Button
                        loading={executing}
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