using CatalogService.Core.Domain.Entities;

namespace CatalogService.Core.Domain.Interfaces
{
    public interface IProductsRepository
    {
        public Task<Product> CreateAsync(Product product);

        public Task<Product> UpdateAsync(Product product);

        public Task<Product?> GetProductByIdAsync(Guid productId);

        public Task<List<Product>> GetProductsAsync(Guid? productId = null, string? categoryName = null, int? page = null, int? pageSize = null);

        public Task<bool> DeleteProductAsync(Product product);
    }
}
