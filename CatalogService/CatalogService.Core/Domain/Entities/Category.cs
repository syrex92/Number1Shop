namespace CatalogService.Core.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public IList<Product> Products { get; set; } = new List<Product>();
    }
}
