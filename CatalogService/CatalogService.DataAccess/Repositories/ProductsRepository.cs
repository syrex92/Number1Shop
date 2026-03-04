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
            return await _dataContext.Products.Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<List<Product>> GetProductsAsync(Guid? categoryId = null, string? categoryName = null, int? page = null, int? pageSize = null)
        {
            var query = _dataContext.Products.Include(p => p.Category).AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            if (!string.IsNullOrEmpty(categoryName))
            {
                query = query.Where(p => p.Category.Name.ToLower() == categoryName.ToLower());
            }

            if (page.HasValue && pageSize.HasValue)
            {
                query = GetProductsWithPaging(query, page.Value, pageSize.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            var res = _dataContext.Products.Update(product);
            await _dataContext.SaveChangesAsync();
            return res.Entity;
        }

        private IQueryable<Product> GetProductsWithPaging(IQueryable<Product> products, int page, int pageSize)
        {
            var skip = (Math.Max(1, page) - 1) * Math.Max(1, pageSize);

            return products
                .OrderByDescending(a => a.UpdatedAt ?? a.CreatedAt)
                .Skip(skip)
                .Take(Math.Max(1, pageSize));
        }
    }
}
