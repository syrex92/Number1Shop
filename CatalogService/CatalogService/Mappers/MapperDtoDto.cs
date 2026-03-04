using CatalogService.Core.Domain.Entities;
using CatalogService.Models;

namespace CatalogService.Mappers
{
    internal static class MapperDtoDto
    {
        internal static ProductDto ToDto(this Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Price = product.Price,
                ProductCategory = product.Category.Name,
                ProductTitle = product.Name,
                StockQuantity = product.StockQuantity,
                ProductDescription = product.Description,
                Article = product.Article,
                ImageUrl = product.ProductImageUrl,
            };
        }

        internal static CategoryDto ToDto(this Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
            };
        }
    }
}
