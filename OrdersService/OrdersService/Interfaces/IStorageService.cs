namespace OrdersService.Interfaces
{
    public interface IStorageService
    {
        public Task<bool> CheckAvailability(Guid productId, int quantity);
        public Task<Guid> Reserve(Models.Order order);
        public Task ConfirmReservation(Guid orderId, Guid reservationId);
        public Task CancelReservation(Guid reservationId);
        public Task<IStockInfo> GetStockInfo(Guid productId);
    }
}