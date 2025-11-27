import { observer } from 'mobx-react-lite';
import { useStores } from '../context/RootStoreContext';
import '../styles/OrdersPage.css';
import { useEffect } from 'react';
import OrderDetails from '../components/OrderDetails';
import { useParams } from 'react-router-dom';

const OrdersDetailsPage = observer(() => {
  let params = useParams();
  const { orders } = useStores();

  const orderId = params.id;
  const order = orders.orders.find(o => o.id === orderId);

  useEffect(() => {
      orders.fetchOrderDetails(orderId ?? '');
    }, []);
  return (
    <div className="orders-page">
      {order === undefined ? (
        <div className="empty">Заказ не найден</div>
      ) : (
        <div className="order-details-container">
          <OrderDetails order={order} />
        </div>
      )}
    </div>
  );
});

export default OrdersDetailsPage;


