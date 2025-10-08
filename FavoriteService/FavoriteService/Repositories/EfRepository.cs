using Microsoft.EntityFrameworkCore;
using Shop.FavoriteService.Abstractions;
using Shop.FavoriteService.Model;

namespace Shop.FavoriteService.Repositories;

/// <inheritdoc />
internal class EfRepository<T>(EfContext context) : IRepository<T> where T : BaseEntity
{
    /// <inheritdoc />
    public Task<T?> GetById(Guid id)
    {
        return context.Set<T>()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <inheritdoc />
    public async Task<T> Add(T item)
    {
        var addedItem = await context.Set<T>().AddAsync(item);
        await context.SaveChangesAsync();
        return addedItem.Entity;
    }

    /// <inheritdoc />
    public async Task<T> Update(T item)
    {
        context.Set<T>().Update(item);
        await context.SaveChangesAsync();
        return item;
    }
}