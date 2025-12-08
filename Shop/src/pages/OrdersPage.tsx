import { observer } from 'mobx-react-lite';
import { useStores } from '../context/RootStoreContext';
import '../styles/OrdersPage.css';
import { useEffect } from 'react';
import OrderCard from '../components/OrderCard';

const OrdersPage = observer(() => {
  const { orders } = useStores();

  useEffect(() => {
        orders.fetchOrders();
    }, []);
  return (
    <div className="orders-page">
      {orders.orders.length === 0 ? (
        <div className="empty">У вас пока нет заказов</div>
      ) : (
        <div className="orders-list">
          { orders.orders.map((o) => (
            <OrderCard order={o} />
          )) }
        </div>
      )}
    </div>
  );
});

export default OrdersPage;


