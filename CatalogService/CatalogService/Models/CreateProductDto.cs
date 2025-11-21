namespace CatalogService.Models
{
    public class CreateProductDto
    {
        public required string ProductTitle {  get; set; }

        public string ProductDescription { get; set; } = string.Empty;

        public required string ProductCategory { get; set; }

        public required int Price { get; set; }

        public required long Article {  get; set; }

        public IList<string> ImagesUrls { get; set; } = [];
    }
}
