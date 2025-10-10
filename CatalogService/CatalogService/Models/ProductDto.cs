namespace CatalogService.Models
{
    public class ProductDto : BaseProduct
    {
        public Guid Id { get; set; }

        public required int StockQuantity { get; set; } = 0;
    }
}
