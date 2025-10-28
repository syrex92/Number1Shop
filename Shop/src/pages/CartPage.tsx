import { observer } from 'mobx-react-lite';
import { useStores } from '../context/RootStoreContext';
import '../styles/CartPage.css';
import CartItemCard from "../components/Cart/CartItemCard.tsx";

const CartPage = observer(() => {
  const { cart } = useStores();

  const items = Array.from(cart.items.values());

  return (
    <div className="cart-page">
      {items.length === 0 ? (
        <div className="empty">Корзина пуста</div>
      ) : (
        <>
          <div className="cart-list">
            {items.map((item, index) => (
              <CartItemCard key={index} cartItem={item} />
            ))}
          </div>
          <div className="cart-footer">
            <div>Итого: {cart.total} ₽</div>
            <button className="checkout" disabled={items.length === 0}>Оформить</button>
          </div>
        </>
      )}
    </div>
  );
});

export default CartPage;


