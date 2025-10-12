using CatalogService.Models;

namespace CatalogService.Interfaces
{
    public interface ICategoriesService
    {
        public Task<IList<ProductDto>?> GetProductsByCategoryIdAsync(Guid categoryId);

        public Task<IList<CategoryDto>> GetAllCategoriesAsync();

    }
}
