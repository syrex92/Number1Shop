using System.Text.Json.Serialization;
using OrdersService.Interfaces;

namespace OrdersService.Models
{
    public class ProductInfo : IProductInfo
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("stockQuantity")]
        public int StockQuantity { get; set; }
        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; } = string.Empty;
        [JsonPropertyName("productTitle")]
        public string ProductTitle { get; set; } = string.Empty;
        [JsonPropertyName("productDescription")]
        public string ProductDescription { get; set; } = string.Empty;
        [JsonPropertyName("productCategory")]
        public string ProductCategory { get; set; } = string.Empty;
        [JsonPropertyName("price")]
        public int Price { get; set; }
        [JsonPropertyName("article")]
        public int Article { get; set; }
    }
}