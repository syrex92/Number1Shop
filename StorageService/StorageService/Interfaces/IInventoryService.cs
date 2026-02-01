using StorageService.DTO;
using StorageService.Models;

namespace StorageService.Interfaces
{
    public interface IInventoryService
    {
        // Проверка доступности
        Task<StockAvailabilityResponse> CheckAvailabilityAsync(Guid productId, int requestedQuantity);

        // Резервирование
        Task<ReservationResponse> ReserveStockAsync(ReservationRequest request);

        // Подтверждение покупки (списание)
        Task<PurchaseConfirmationResponse> ConfirmPurchaseAsync(PurchaseConfirmationRequest request);

        // Отмена резервирования
        Task<bool> CancelReservationAsync(Guid reservationId);

        // Получение информации о товаре на складе
        Task<StockItem?> GetStockItemAsync(Guid productId);
    }
}
