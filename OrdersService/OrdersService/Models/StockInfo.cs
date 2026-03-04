using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OrdersService.Models
{
    public class StockInfo: Interfaces.IStockInfo
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("productId")]
        public Guid ProductId { get; set; }
        [JsonPropertyName("totalQuantity")]
        public int TotalQuantity { get; set; }
        [JsonPropertyName("sku")]
        public Guid Sku { get; set; }
        [JsonPropertyName("manufactureDate")]
        public string ManufactureDate { get; set; } = string.Empty;
        [JsonPropertyName("purchasePrice")]
        public decimal PurchasePrice { get; set; }
        [JsonPropertyName("supplierId")]
        public Guid SupplierId { get; set; }
        [JsonPropertyName("condition")]
        public int Condition { get; set; }
        [JsonPropertyName("createdAt")]
        public string CreatedAt { get; set; } = string.Empty;
        [JsonPropertyName("updatedAt")]
        public string UpdatedAt { get; set; } = string.Empty;
        [JsonPropertyName("error")]
        public string Error { get; set; } = string.Empty;
    }
}