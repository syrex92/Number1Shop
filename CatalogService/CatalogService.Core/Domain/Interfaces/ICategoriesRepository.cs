using CatalogService.Core.Domain.Entities;

namespace CatalogService.Core.Domain.Interfaces
{
    public interface ICategoriesRepository
    {
        public Task<Category> CreateAsync(Category product);

        public Task<Category> UpdateAsync(Category product);

        public Task<Category?> GetCategoryByIdAsync(Guid categoryId);

        public Task<Category?> GetCategoryByNameAsync(string name);

        public Task<List<Category>> GetCategoriesAsync();

        public Task<bool> DeleteCategoryAsync(Category category);
    }
}
