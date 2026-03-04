namespace CatalogService.Core.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public string Description { get; set; } = string.Empty;

        public Guid CategoryId { get; set; }

        public Category Category { get; set; }

        public int Price { get; set; }

        public int StockQuantity { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public long Article { get; set; }

        public string? ProductImageUrl { get; set; }
    }
}
