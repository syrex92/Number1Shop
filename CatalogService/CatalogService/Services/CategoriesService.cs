using CatalogService.Core.Domain.Interfaces;
using CatalogService.Interfaces;
using CatalogService.Mappers;
using CatalogService.Models;

namespace CatalogService.Services
{
    internal class CategoriesService : ICategoriesService
    {
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly IProductsRepository _productsRepository;
        public CategoriesService(IProductsRepository productsRepository, ICategoriesRepository categoriesRepository)
        {
            _categoriesRepository = categoriesRepository;
            _productsRepository = productsRepository;
        }

        public async Task<IList<CategoryDto>> GetAllCategoriesAsync()
        {
            return (await _categoriesRepository.GetCategoriesAsync()).Select(c => c.ToDto()).ToList();
        }

        public async Task<IList<ProductDto>?> GetProductsByCategoryIdAsync(Guid categoryId, int? page = null, int? pageSize = null)
        {
            var category = await _categoriesRepository.GetCategoryByIdAsync(categoryId);
            if (category == null) { return null; }



            return (await _productsRepository.GetProductsAsync(categoryId, page: page, pageSize: pageSize)).Select(p => p.ToDto()).ToList();
        }
    }
}
