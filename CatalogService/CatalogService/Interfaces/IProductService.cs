using CatalogService.Models;

namespace CatalogService.Interfaces
{
    public interface IProductService
    {
        public Task<ProductDto> CreateProductAsync(CreateProductDto createDto);

        public Task<ProductDto?> UpdateImageAsync(Guid id, IFormFile file);

        public Task<ProductDto?> GetProductByIdAsync(Guid productId);

        public Task<IList<ProductDto>> GetAllProductsAsync(int? page = null, int? pageSize = null);

        public Task<ProductDto?> UpdateProductAsync(Guid productId, UpdateProductDto updateDto);

        public Task<bool> DeleteProductAsync(Guid productId);
    }
}
