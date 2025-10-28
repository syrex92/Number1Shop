import {observer} from 'mobx-react-lite';
import {useStores} from '../context/RootStoreContext';
import '../styles/CartPage.css';
import CartItemCard from "../components/Cart/CartItemCard.tsx";
import CartItemsFooter from "../components/Cart/CartItemsFooter.tsx";
import CartItemsHeader from "../components/Cart/CartItemsHeader.tsx";

const CartPage = observer(() => {
    const {cart} = useStores();

    const items = Array.from(cart.items.values());

    return (
        <div className="cart-page">
            {items.length === 0 ? (
                <div className="empty">Корзина пуста</div>
            ) : (
                <>
                    <CartItemsHeader />
                    <div className="cart-list">
                        {items.map((item, index) => (
                            <CartItemCard key={index} cartItem={item}/>
                        ))}
                    </div>
          
                    <CartItemsFooter/>
                </>
            )}
        </div>
    );
});

export default CartPage;


