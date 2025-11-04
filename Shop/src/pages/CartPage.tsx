import {observer} from 'mobx-react-lite';
import {useStores} from '../context/RootStoreContext';
import '../styles/CartPage.css';
import CartItemCard from "../components/Cart/CartItemCard.tsx";
import CartItemsFooter from "../components/Cart/CartItemsFooter.tsx";
import CartItemsHeader from "../components/Cart/CartItemsHeader.tsx";
import {useEffect} from "react";

const CartPage = observer(() => {
    const {cart} = useStores();

    const items = Array.from(cart.items.values());

    useEffect(() => {
        cart.fetchItems()
    }, []);

    function CartContent() {
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

    function CartLoading() {
        return (
            <div className="empty">LOADING</div>
        )
    }

    return (
        <div className="cart-page">
            {
                cart.loading ? <CartLoading/> :
                    items.length === 0 ? (
                        <EmptyCart/>
                    ) : (
                        <CartContent/>
                    )}
        </div>
    );
});

export default CartPage;


