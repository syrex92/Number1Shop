namespace StorageService.Models
{
    public class StockReservation
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; } // ID заказа из OrderService

        public ReservationStatus Status { get; set; } = ReservationStatus.Reserved;

        // Срок действия резерва
        public DateTime ReservedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public DateTime? ConfirmedAt { get; set; } // Когда подтверждена покупка
        public DateTime? CancelledAt { get; set; } // Когда отменен

        // Детали
        public List<ReservationDetail> Details { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Параметры резерва товара одного наименования
    /// </summary>

    public class ReservationDetail
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ReservationId { get; set; }
        public Guid ProductId { get; set; }
        public Guid StockItemId { get; set; } // Конкретная складская позиция

        public int Quantity { get; set; }
        public decimal ReservedPrice { get; set; } // Цена на момент резервирования

        // Статусы
        public bool IsConfirmed { get; set; } = false;
        public DateTime? ConfirmedAt { get; set; }

        // Для отмены/возврата
        public bool IsCancelled { get; set; } = false;
        public DateTime? CancelledAt { get; set; }
        public string? CancellationReason { get; set; }
    }

    public enum ReservationStatus
    {
        Reserved = 1,        // Зарезервировано
        Confirmed = 2,       // Подтверждено (куплено)
        Expired = 3,         // Истек срок
        Cancelled = 4,       // Отменено
    }
}
