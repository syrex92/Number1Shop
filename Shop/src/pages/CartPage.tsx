import {observer} from 'mobx-react-lite';
import {useStores} from '../context/RootStoreContext';
import '../styles/CartPage.css';
import CartItemCard from "../components/Cart/CartItemCard.tsx";
import CartItemsFooter from "../components/Cart/CartItemsFooter.tsx";
import CartItemsHeader from "../components/Cart/CartItemsHeader.tsx";
import {useEffect} from "react";
import {Button, Center, Stack, Text} from "@mantine/core";
import {logger} from "../utils/logger.ts";

const CartPage = observer(() => {
    const {cart} = useStores();

    useEffect(() => {
        console.log(cart.items)
        cart.fetchItems().catch(error => {
            logger.error(error);
        })
    }, []);

    function FilledCart() {
        return (
            <>
                <CartItemsHeader/>
                <div className="cart-list">
                    {Array.from(cart.items.values()).map((item) => (
                        <CartItemCard key={item.product.id} cartItem={item}/>
                    ))}
                </div>

                <CartItemsFooter/>
            </>
        )
    }

    function EmptyCart() {
        return (
            <div className="empty">Корзина пуста</div>
        )
    }

    const CartLoading = () => {
        
        return (
            <div className="empty">Загружаем корзину...</div>
        )
    }

    const CartError = () => {
        return (
            <Center>
                <Stack>
                    <Text fw={500}>Что-то пошло не так...</Text>
                    <Text c="red" fw={200}>{cart.errorText}</Text>
                    <Button loading={cart.loading} 
                            onClick={() => { cart.fetchItems() }}>
                        Повторить
                    </Button>
                </Stack>
            </Center>
        )
    }

    const CartContent = () => {
        if (cart.error) {
            return <CartError/>
        } else if (cart.loading) {
            return <CartLoading/>
        } else if (cart.items.size === 0) {
            return <EmptyCart/>
        }
        return <FilledCart/>
    }

    return (
        <div className="cart-page">
            <CartContent/>
        </div>
    );
});

export default CartPage;


