using Shop.Demo.Data;
using StorageService.Models;

namespace StorageService
{
    public class FakeStorageData
    {
        /// <summary>
        /// Поставщики
        /// </summary>
        public static List<Supplier> Suppliers { get; } = ShopFakeData.Suppliers.Select(x => new Supplier
        {
            Id = x.Id,
            Name = x.Name,
            ContactPerson = x.ContactPerson,
            Email = x.Email,
            Phone = x.Phone,
            Address = x.Address,
            City = x.Address,
            Country = x.Country,
            INN = x.INN,
            BankDetails = x.BankDetails,
            AverageDeliveryDays = x.AverageDeliveryDays,
            MinimumOrderAmount = x.MinimumOrderAmount,
            Rating = SupplierRating.Good,
            IsActive = x.IsActive,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }).ToList();

        /// <summary>
        /// Склад
        /// </summary>
        public static List<StockItem> Stocks { get; } = ShopFakeData.Stocks.Select(x => new StockItem
        {
            Id = x.Id,
            ProductId = x.ProductId,
            TotalQuantity = x.TotalQuantity,
            SKU = Guid.NewGuid().ToString(),
            ManufactureDate = x.ManufactureDate,
            PurchasePrice = x.PurchasePrice,
            SupplierId = x.SupplierId,
            Condition = StockItemCondition.New,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt,
        }).ToList();
    }
}
