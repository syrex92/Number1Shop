import { useNavigate } from "react-router-dom";
import type { Order } from "../stores/OrdersStore";
import {Button} from "@mantine/core";
import { useStores } from "../context/RootStoreContext";

const OrderCard = (props: {order: Order}) => {
    const { order } = props;
    const { orders } = useStores();
    const navigate = useNavigate();

    function handleClickEditBtn(event: React.MouseEvent<HTMLButtonElement>) {
        event.preventDefault();
        navigate(`/orders/${order.id}`);
    }

    function handleClickRemoveBtn(event: React.MouseEvent<HTMLButtonElement>) {
        event.preventDefault();
        orders.cancelOrder(event.currentTarget.dataset.id ?? "");
    }

    return <div key={order.id} className="order-card">
            <div className="order-card-header">Заказ #{order.orderNumber}</div>
            <div className="order-card-body">
                <div>Дата создания: {order.createdAt}</div>
                <div>Адрес доставки: {order.deviveryAddress}</div>
                <div>Статус: {order.status}</div>
                <div>Итоговая сумма: {order.totalPrice} ₽</div>
            </div>
            <div className="order-card-footer">
                <Button onClick={handleClickEditBtn}>Детали заказа</Button>
                {(order.status === 'New' || order.status === 'Processing') && (
                    <>
                        <Button color="red" data-id={order.id} onClick={handleClickRemoveBtn}>Отменить</Button>
                    </>
                )}
            </div>
        </div>;
};

export default OrderCard;