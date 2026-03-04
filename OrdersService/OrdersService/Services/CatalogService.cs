using Microsoft.Extensions.Caching.Distributed;
using OrdersService.Interfaces;
using OrdersService.Models;

namespace OrdersService.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _cache;

        public CatalogService(IHttpClientFactory httpClientFactory, IDistributedCache cache)
        {
            _httpClient = httpClientFactory.CreateClient("CatalogService");
            _cache = cache;
        }
        public async Task<IProductInfo> GetById(Guid productId)
        {
            var response = await _httpClient.GetAsync($"/Products/{productId}");
            try
            {
                response.EnsureSuccessStatusCode();
            } catch
            {
                throw new Exception($"Failed to get product info for product {productId}. Status code: {response.StatusCode}");
            }
            var productInfo = await response.Content.ReadFromJsonAsync<ProductInfo>();
            if (productInfo == null)
            {
                throw new Exception("Invalid response from catalog service");
            }
            return productInfo;
        }
        public async Task<string> GetTitleByIdWithCache(Guid productId)
        {
            var cacheKey = $"product_{productId}";
            var cachedProductTitle = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedProductTitle))
            {
                return cachedProductTitle;
            }
            var productInfo = await GetById(productId);
            await _cache.SetStringAsync(cacheKey, productInfo.ProductTitle, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(30),
            });
            return productInfo.ProductTitle;
        }
    }
}