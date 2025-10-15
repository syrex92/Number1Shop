namespace CatalogService.Models
{
    public class CategoryDto
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }
    }
}
