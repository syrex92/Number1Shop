namespace StorageService.Models
{
    /// <summary>
    /// Поставщик товаров на склад
    /// </summary>
    public class Supplier
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }

        // Контактная информация
        public string? ContactPerson { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        // Адрес
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }

        // Реквизиты
        public string? INN { get; set; }               // ИНН
        public string? BankDetails { get; set; }

        // Логистика
        public int AverageDeliveryDays { get; set; } = 7;
        public decimal? MinimumOrderAmount { get; set; }

        // Рейтинг и статус
        public SupplierRating Rating { get; set; } = SupplierRating.Good;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public enum SupplierRating
    {
        Excellent = 5,   // Отличный
        Good = 4,        // Хороший
        Average = 3,     // Средний
        Poor = 2,        // Плохой
        Critical = 1     // Критический
    }
}
