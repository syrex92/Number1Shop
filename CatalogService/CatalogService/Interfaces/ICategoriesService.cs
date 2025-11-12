using CatalogService.Models;

namespace CatalogService.Interfaces
{
    public interface ICategoriesService
    {
        public Task<IList<ProductDto>?> GetProductsByCategoryIdAsync(Guid categoryId, int? page = null, int? pageSize = null);

        public Task<IList<CategoryDto>> GetAllCategoriesAsync();

    }
}
