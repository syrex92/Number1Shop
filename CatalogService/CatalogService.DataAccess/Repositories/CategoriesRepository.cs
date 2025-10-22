using CatalogService.Core.Domain.Entities;
using CatalogService.Core.Domain.Interfaces;
using CatalogService.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.DataAccess.Repositories
{
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly DataContext _dataContext;
        public CategoriesRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Category> CreateAsync(Category category)
        {
            var res = await _dataContext.Categories.AddAsync(category);
            await _dataContext.SaveChangesAsync();

            return res.Entity;
        }

        public async Task<bool> DeleteCategoryAsync(Category category)
        {
            _dataContext.Categories.Remove(category);
            await _dataContext.SaveChangesAsync();
            return true;
        }

        public async Task<Category?> GetCategoryByIdAsync(Guid categoryId)
        {
            return await _dataContext.Categories.FirstOrDefaultAsync(p => p.Id == categoryId);
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _dataContext.Categories.ToListAsync();
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            var res = _dataContext.Categories.Update(category);
            await _dataContext.SaveChangesAsync();
            return res.Entity;
        }

        public async Task<Category?> GetCategoryByNameAsync(string name)
        {
            return await _dataContext.Categories.FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
        }
    }
}
