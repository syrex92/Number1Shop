namespace StorageService.DTO
{
    /// <summary>
    /// Запрос на бронь товара
    /// </summary>
    public class ReservationRequest
    {
        public Guid OrderId { get; set; }
        public List<OrderItemRequest> Items { get; set; } = new();
        public DateTime ReservationExpiry { get; set; } = DateTime.UtcNow.AddMinutes(30); // Резерв на 10 минут
    }

    /// <summary>
    /// Параметры запроса на бронь
    /// </summary>
    public class OrderItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class PurchaseConfirmationRequest
    {
        public Guid OrderId { get; set; }
        public List<Guid> ReservationIds { get; set; } = new(); // ID резервирований для подтверждения
    }

    /// <summary>
    /// Проверка достпуности товара для заказа
    /// </summary>
    public class StockAvailabilityResponse
    {
        public bool IsAvailable { get; set; }
        public int AvailableQuantity { get; set; }
        public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
        public Guid ProductId { get; set; }
        public int RequestedQuantity { get; set; }
    }

    public class ReservationResponse
    {
        public bool Success { get; set; }
        public Guid ReservationId { get; set; }
        public DateTime ExpiresAt { get; set; }
        public List<ReservedItem> ReservedItems { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }

    public class ReservedItem
    {
        public Guid ProductId { get; set; }
        public int ReservedQuantity { get; set; }
        public Guid ReservationDetailId { get; set; }
    }

    public class PurchaseConfirmationResponse
    {
        public bool Success { get; set; }
        public Guid OrderId { get; set; }
        public List<ConfirmedItem> ConfirmedItems { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }

    public class ConfirmedItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
