using CatalogService.Api.Models;

namespace CatalogService.Api.Interfaces
{
    public interface ICategoriesService
    {
        public Task<IList<ProductDto>?> GetProductsByCategoryIdAsync(Guid categoryId);

        public Task<IList<CategoryDto>> GetAllCategoriesAsync();

    }
}
