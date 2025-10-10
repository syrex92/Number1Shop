namespace CatalogService.Models
{
    public class CategoryDto
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public IList<ProductDto> Products { get; set; } = new List<ProductDto>();
    }
}
