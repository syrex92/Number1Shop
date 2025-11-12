using CatalogService.Core.Domain.Entities;
using CatalogService.Core.Domain.Interfaces;
using CatalogService.Interfaces;
using CatalogService.Mappers;
using CatalogService.Models;

namespace CatalogService.Services
{
    internal class ProductsService : IProductService
    {
        private readonly IProductsRepository _productsRepository;
        private readonly ICategoriesRepository _categoriesRepository;
        public ProductsService(IProductsRepository productsRepository, ICategoriesRepository categoriesRepository)
        {
            _productsRepository = productsRepository;
            _categoriesRepository = categoriesRepository;
        }

        public async Task<ProductDto> CreateProductAsync(CreateOrUpdateProductDto createDto)
        {
            var existCategory = await _categoriesRepository.GetCategoryByNameAsync(createDto.ProductCategory);

            var res = await _productsRepository.CreateAsync(new Product
            {
                Name = createDto.ProductTitle,
                Description = createDto.ProductDescription,
                Category = existCategory ?? new Category { Name = createDto.ProductCategory },
                CreatedAt = DateTime.UtcNow,
                Price = createDto.Price,
                ProductImages = createDto.ImagesUrls.Select(i => new ProductImage { ImageUrl =  i.ToString() }).ToList(),
            });

            return res.ToDto();
        }

        public async Task<bool> DeleteProductAsync(Guid productId)
        {
            var existProduct = await _productsRepository.GetProductByIdAsync(productId);
            if (existProduct == null) { return false; }

            await _productsRepository.DeleteProductAsync(existProduct);

            var categoryOfProduct = await _categoriesRepository.GetCategoryByNameAsync(existProduct.Category.Name);

            if (categoryOfProduct != null && categoryOfProduct.Products.Count == 0)
            {
                await _categoriesRepository.DeleteCategoryAsync(categoryOfProduct);
            }

            return true;
        }

        public async Task<IList<ProductDto>> GetAllProductsAsync(int? page = null, int? pageSize = null)
        {
            return (await _productsRepository.GetProductsAsync(page:page, pageSize:pageSize)).Select(p => p.ToDto()).ToList();
        }

        public async Task<ProductDto?> GetProductByIdAsync(Guid productId)
        {
            return (await _productsRepository.GetProductByIdAsync(productId))?.ToDto();
        }

        public async Task<ProductDto?> UpdateProductAsync(Guid productId, CreateOrUpdateProductDto updateDto)
        {
            var existProduct = await _productsRepository.GetProductByIdAsync(productId);

            if (existProduct == null) { return null; }

            if (updateDto.ImagesUrls.Any())
            {
                existProduct.ProductImages.Clear();
                existProduct.ProductImages = updateDto.ImagesUrls.Select(i => new ProductImage { ImageUrl = i.ToString() }).ToList();
            }

            existProduct.UpdatedAt = DateTime.UtcNow;
            existProduct.Price = updateDto.Price;
            existProduct.Name = updateDto.ProductTitle;

            if (!string.IsNullOrEmpty(updateDto.ProductDescription))
            {
                existProduct.Description = updateDto.ProductDescription;
            }

            var existCategory = await _categoriesRepository.GetCategoryByNameAsync(updateDto.ProductCategory);

            if (existCategory == null)
            {
                existCategory = await _categoriesRepository.CreateAsync(new Category { Name = updateDto.ProductCategory });
            }

            var oldCategoryName = existProduct.Category.Name;
            existProduct.Category = existCategory;

            var res = await _productsRepository.UpdateAsync(existProduct);

            var oldCategory = await _categoriesRepository.GetCategoryByNameAsync(oldCategoryName);

            if (oldCategory != null && oldCategory.Products.Count == 0)
            {
                await _categoriesRepository.DeleteCategoryAsync(oldCategory);
            }

            return res.ToDto();
        }
    }
}
