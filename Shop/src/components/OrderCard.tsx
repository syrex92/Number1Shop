import { useNavigate } from "react-router-dom";
import type { Order } from "../stores/OrdersStore";
import {Button} from "@mantine/core";

const OrderCard = (props: {order: Order}) => {
    const { order } = props;
    const navigate = useNavigate();

    function handleClickEditBtn(event: React.MouseEvent<HTMLButtonElement>) {
        event.preventDefault();
        navigate(`/orders/${order.id}`);
    }

    function handleClickRemoveBtn(event: React.MouseEvent<HTMLButtonElement>) {
        event.preventDefault();
        alert("Отмена заказа пока не реализована");
    }

    return <div key={order.id} className="order-card">
            <div className="order-card-header">Заказ #{order.id}</div>
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
                        <Button color="red" onClick={handleClickRemoveBtn}>Отменить</Button>
                    </>
                )}
            </div>
        </div>;
};

export default OrderCard;