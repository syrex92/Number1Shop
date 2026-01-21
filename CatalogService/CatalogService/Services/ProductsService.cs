using CatalogService.Configurations;
using CatalogService.Core.Domain.Entities;
using CatalogService.Core.Domain.Interfaces;
using CatalogService.Interfaces;
using CatalogService.Mappers;
using CatalogService.Models;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Shop.Core.Messages;

namespace CatalogService.Services
{
    internal class ProductsService : IProductService
    {
        private readonly IProductsRepository _productsRepository;
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly IImageStorage _imageStorage;
        private readonly IBusControl _busControl;
        private readonly string? _queueForSendMessage;

        public ProductsService(IProductsRepository productsRepository, ICategoriesRepository categoriesRepository, IBusControl busControl, IConfiguration configuration, IImageStorage imageStorage)
        {
            _productsRepository = productsRepository;
            _categoriesRepository = categoriesRepository;
            _busControl = busControl;
            _imageStorage = imageStorage;
            var rmqSettings = configuration.GetSection("RabbitMqConfiguration").Get<RabbitMqConfiguration>();
            _queueForSendMessage = configuration["RMQ_PRODUCT_FROM_CATALOG_QUEUE"] ?? rmqSettings?.ProductsFromCatalogQueue;

            if (string.IsNullOrEmpty(_queueForSendMessage))
            {
                _queueForSendMessage = "catalog-product-messages-queue";
            }
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createDto)
        {
            var existCategory = await _categoriesRepository.GetCategoryByNameAsync(createDto.ProductCategory);

            string? imageUrl = null;
            if (createDto.Image != null)
            {
                try
                {
                    imageUrl = await _imageStorage.SaveAsync(createDto.Image);
                }
                catch (Exception)
                {
                    //TODO:log
                }
            }

            var res = await _productsRepository.CreateAsync(new Product
            {
                Name = createDto.ProductTitle,
                Description = createDto.ProductDescription,
                Category = existCategory ?? new Category { Name = createDto.ProductCategory },
                CreatedAt = DateTime.UtcNow,
                Price = createDto.Price,
                Article = createDto.Article,
                ProductImageUrl = imageUrl,
            });

            var sendEndpoint = await _busControl.GetSendEndpoint(new Uri($"queue:{_queueForSendMessage}"));

            await sendEndpoint.Send(new CatalogProductMessage
            {
                Article = res.Article,
                EventType = CatalogProductEventType.ProductAddedToCatalog,
                Price = res.Price,
                ProductId = res.Id,
                Title = res.Name
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

            var sendEndpoint = await _busControl.GetSendEndpoint(new Uri($"queue:{_queueForSendMessage}"));

            await sendEndpoint.Send(new CatalogProductMessage
            {
                Article = existProduct.Article,
                EventType = CatalogProductEventType.ProductRemovedFromCatalog,
                ProductId = existProduct.Id,
            });

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

        public async Task<ProductDto?> UpdateImageAsync(Guid id, IFormFile file)
        {
            var existProduct = await _productsRepository.GetProductByIdAsync(id);
            if (existProduct == null) { return null; }

            try
            {
                var oldImageUrl = existProduct.ProductImageUrl;
                var imageUrl = await _imageStorage.SaveAsync(file);

                existProduct.UpdatedAt = DateTime.Now;
                existProduct.ProductImageUrl = imageUrl;
                var res = await _productsRepository.UpdateAsync(existProduct);

                try
                {
                    if (oldImageUrl != null)
                    {
                        _imageStorage.DeleteFile(oldImageUrl);
                    }

                }
                catch (Exception ex)
                {
                    //TODO: log
                }

                return res.ToDto();
            }
            catch (Exception)
            {
                //TODO: logic
                return null;
            }
        }

        public async Task<ProductDto?> UpdateProductAsync(Guid productId, UpdateProductDto updateDto)
        {
            var existProduct = await _productsRepository.GetProductByIdAsync(productId);

            if (existProduct == null) { return null; }

            existProduct.UpdatedAt = DateTime.UtcNow;
            existProduct.Price = updateDto.Price;
            existProduct.Article = updateDto.Article;
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

            var sendEndpoint = await _busControl.GetSendEndpoint(new Uri($"queue:{_queueForSendMessage}"));

            await sendEndpoint.Send(new CatalogProductMessage
            {
                Article = res.Article,
                EventType = CatalogProductEventType.ProductChangedInCatalog,
                ProductId = res.Id,
                Price = res.Price,
                Title = res.Name
            });

            return res.ToDto();
        }
    }
}
