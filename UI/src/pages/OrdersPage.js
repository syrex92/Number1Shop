import React from 'react';
import { observer } from 'mobx-react-lite';
import { useStores } from '../context/RootStoreContext';
import '../styles/OrdersPage.css';

const OrdersPage = observer(() => {
  const { orders } = useStores();
  return (
    <div className="orders-page">
      {orders.orders.length === 0 ? (
        <div className="empty">У вас пока нет заказов</div>
      ) : (
        orders.orders.map((o) => (
          <div key={o.id} className="order-card">Заказ #{o.id}</div>
        ))
      )}
    </div>
  );
});

export default OrdersPage;


