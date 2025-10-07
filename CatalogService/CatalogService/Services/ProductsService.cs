using CatalogService.Api.Interfaces;
using CatalogService.Api.Models;

namespace CatalogService.Api.Services
{
    public class ProductsService : IProductService
    {
        public ProductsService()
        {
            
        }

        public Task<ProductDto> CreateProductAsync(CreateOrUpdateProductDto createDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteProductAsync(Guid productId)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProductDto>> GetAllProductsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ProductDto> GetProductByIdAsync(Guid productId)
        {
            throw new NotImplementedException();
        }

        public Task<ProductDto> UpdateProductAsync(Guid productId, CreateOrUpdateProductDto updateDto)
        {
            throw new NotImplementedException();
        }
    }
}
