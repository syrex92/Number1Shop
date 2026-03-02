using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersService.Interfaces
{
    public interface IStockInfo
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int TotalQuantity { get; set; }
        public Guid Sku { get; set; }
        public string ManufactureDate { get; set; }
        public decimal PurchasePrice { get; set; }
        public Guid SupplierId { get; set; }
        public int Condition { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Error { get; set; }
    }
}