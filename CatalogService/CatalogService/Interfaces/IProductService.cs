using CatalogService.Models;

namespace CatalogService.Interfaces
{
    public interface IProductService
    {
        public Task<ProductDto> CreateProductAsync(CreateOrUpdateProductDto createDto);

        public Task<ProductDto?> GetProductByIdAsync(Guid productId);

        public Task<IList<ProductDto>> GetAllProductsAsync();

        public Task<ProductDto?> UpdateProductAsync(Guid productId, CreateOrUpdateProductDto updateDto);

        public Task<bool> DeleteProductAsync(Guid productId);
    }
}
