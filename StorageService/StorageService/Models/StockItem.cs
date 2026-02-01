namespace StorageService.Models
{
    /// <summary>
    /// Складская позиция
    /// </summary>
    public class StockItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Связь с товаром
        public Guid ProductId { get; set; }

        // Количество
        public int TotalQuantity { get; set; }               // Всего на складе

        // Логистика
        public string SKU { get; set; } = string.Empty;     // Складской SKU (отличается от артикула)
        public DateTime? ManufactureDate { get; set; }       // Дата производства

        // Стоимость и поставщик
        public decimal PurchasePrice { get; set; }          // Закупочная цена
        public Guid? SupplierId { get; set; }               // Поставщик

        // Состояние
        public StockItemCondition Condition { get; set; } = StockItemCondition.New;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public enum StockItemCondition
    {
        New = 1,            // Новый
        Refurbished = 2,    // Восстановленный
        Used = 3,           // Б/у
        Demo = 4,           // Демо-образец
        OpenBox = 5         // Распакованный
    }
}
