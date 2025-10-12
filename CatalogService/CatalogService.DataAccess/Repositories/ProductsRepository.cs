using CatalogService.Core.Domain.Entities;
using CatalogService.Core.Domain.Interfaces;
using CatalogService.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.DataAccess.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly DataContext _dataContext;
        public ProductsRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            var res = await _dataContext.Products.AddAsync(product);
            await _dataContext.SaveChangesAsync();

            return res.Entity;
        }

        public async Task<bool> DeleteProductAsync(Product product)
        {
            _dataContext.Products.Remove(product);
            await _dataContext.SaveChangesAsync();

            return true;
        }

        public async Task<Product?> GetProductByIdAsync(Guid productId)
        {
            return await _dataContext.Products.Include(p => p.ProductImages).Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<List<Product>> GetProductsAsync(int? page, int? pageSize)
        {
            if (page.HasValue && pageSize.HasValue)
            {
                return await GetProductsWithPaging(page.Value, pageSize.Value).ToListAsync();
            }

            return await _dataContext.Products.Include(p => p.ProductImages).Include(p => p.Category).ToListAsync();
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(string categoryName, int? page, int? pageSize)
        {
            if (page.HasValue && pageSize.HasValue)
            {
                var query = GetProductsWithPaging(page.Value, pageSize.Value);

                return await query.Where(p => p.Category.Name.ToLower() == categoryName.ToLower()).ToListAsync();
            }

            return await _dataContext.Products.Include(p => p.ProductImages).Include(p => p.Category).ToListAsync();
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            var res = _dataContext.Products.Update(product);
            await _dataContext.SaveChangesAsync();
            return res.Entity;
        }

        private IQueryable<Product> GetProductsWithPaging(int page, int pageSize)
        {
            var skip = (Math.Max(1, page) - 1) * Math.Max(1, pageSize);

            return _dataContext.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .OrderByDescending(a => a.UpdatedAt ?? a.CreatedAt)
                .Skip(skip)
                .Take(Math.Max(1, pageSize));
        }
    }
}
