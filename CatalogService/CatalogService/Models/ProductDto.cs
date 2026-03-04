namespace CatalogService.Models
{
    public class ProductDto : CreateProductDto
    {
        public Guid Id { get; set; }

        public required int StockQuantity { get; set; } = 0;

        public string? ImageUrl { get; set; }
    }
}
