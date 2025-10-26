import { observer } from 'mobx-react-lite';
import { useStores } from '../context/RootStoreContext';
import '../styles/CartPage.css';

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
            {items.map(({ product, qty }) => (
              <div key={product.id} className="cart-row">
                <div className="title">{product.title}</div>
                <div className="qty">
                  <button onClick={() => cart.decrease(product.id)}>-</button>
                  <span>{qty}</span>
                  <button onClick={() => cart.add(product)}>+</button>
                </div>
                <div className="price">{product.price * qty} ₽</div>
                <button className="remove" onClick={() => cart.remove(product.id)}>×</button>
              </div>
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


