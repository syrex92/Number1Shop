using CatalogService.Api.Models;
using CatalogService.Core.Domain.Entities;

namespace CatalogService.Api.Mappers
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
                ImagesUrls = product.ProductImages.Select(pi => pi.ImageUrl).ToList(),
            };
        }

        internal static CategoryDto ToDto(this Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Products = category.Products.Select(p => p.ToDto()).ToList()
            };
        }
    }
}
