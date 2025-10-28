using Microsoft.EntityFrameworkCore;
using Shop.FavoriteService.Abstractions;
using Shop.FavoriteService.Model;

namespace Shop.FavoriteService.Repositories;

/// <inheritdoc />
internal class ProductSearchRepository(EfContext context) : IProductSearchRepository
{
    public async Task<IEnumerable<FavoriteItem>> GetByProductId(Guid productId)
    {
        var items = await context.Set<FavoriteItem>().Where(x => x.ProductId == productId).ToListAsync();
        return items;
    }
}