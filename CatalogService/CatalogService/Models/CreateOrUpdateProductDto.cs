namespace CatalogService.Models
{
    public class UpdateProductDto
    {
        public string ProductTitle { get; set; }

        public string ProductDescription { get; set; }

        public string ProductCategory { get; set; }

        public int Price { get; set; }

        public long Article { get; set; }

        public IList<string> ImagesUrls { get; set; } = [];
    }
}
