import { useStores } from "../context/RootStoreContext";
import type { Order } from "../stores/OrdersStore";
import {Button} from "@mantine/core";

const OrderDetails = (props: {order: Order}) => {
    const { order } = props;
    const { orders } = useStores();

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
            <div className="order-card-products">
                <h3>Товары в заказе:</h3>
                {(() => {
                    const items = (order as Order).items ?? [];
                    if (!items.length) return <p>Заказ пуст</p>;

                    return (
                        <table className="order-items-table">
                            <thead>
                                <tr>
                                    <th>Наименование товара</th>
                                    <th>Количество</th>
                                    <th>Цена</th>
                                    <th>Сумма</th>
                                </tr>
                            </thead>
                            <tbody>
                                {items.map((it: any, i: number) => (
                                    <tr key={it.id ?? i}>
                                        <td>{it.name ?? '—'}</td>
                                        <td>{it.quantity ?? 1}</td>
                                        <td>{it.cost ?? 0}</td>
                                        <td>{it.quantity * it.cost}</td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    );
                })()}
            </div>
            <div className="order-card-footer">
                {(order.status === 'New' || order.status === 'Processing') && (
                    <>
                        <Button color="red" data-id={order.id} onClick={handleClickRemoveBtn}>Отменить</Button>
                    </>
                )}
            </div>
        </div>;
};

export default OrderDetails;