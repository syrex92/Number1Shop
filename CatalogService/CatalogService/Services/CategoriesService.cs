using CatalogService.Core.Domain.Interfaces;
using CatalogService.Interfaces;
using CatalogService.Mappers;
using CatalogService.Models;

namespace CatalogService.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly ICategoriesRepository _categoriesRepository;
        public CategoriesService(IProductsRepository productsRepository, ICategoriesRepository categoriesRepository)
        {
            _categoriesRepository = categoriesRepository;
        }

        public async Task<IList<CategoryDto>> GetAllCategoriesAsync()
        {
            return (await _categoriesRepository.GetCategoriesAsync()).Select(c => c.ToDto()).ToList();
        }

        public async Task<IList<ProductDto>?> GetProductsByCategoryIdAsync(Guid categoryId)
        {
            var category = await _categoriesRepository.GetCategoryByIdAsync(categoryId);
            if (category == null) { return null; }
            return category.Products.Select(p => p.ToDto()).ToList();
        }
    }
}
