import {observer} from 'mobx-react-lite';
import {useStores} from '../context/RootStoreContext';
import '../styles/CartPage.css';
import CartItemCard from "../components/Cart/CartItemCard.tsx";
import CartItemsFooter from "../components/Cart/CartItemsFooter.tsx";
import CartItemsHeader from "../components/Cart/CartItemsHeader.tsx";
import {useEffect} from "react";
import {Button, Center, Stack, Text} from "@mantine/core";

const CartPage = observer(() => {
    const {cart} = useStores();

    const items = Array.from(cart.items.values());

    useEffect(() => {
        cart.fetchItems()
    }, []);

    function FilledCart() {
        return (
            <>
                <CartItemsHeader/>
                <div className="cart-list">
                    {items.map((item, index) => (
                        <CartItemCard key={index} cartItem={item}/>
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

    const CartContent = observer(() => {
        if (cart.error) {
            return <CartError/>
        } else if (cart.loading) {
            return <CartLoading/>
        } else if (items.length === 0) {
            return <EmptyCart/>
        }
        return <FilledCart/>
    })

    return (
        <div className="cart-page">
            <CartContent/>
        </div>
    );
});

export default CartPage;


