using Microsoft.EntityFrameworkCore;
using Shop.CartService.Abstractions;
using Shop.CartService.Model;

namespace Shop.CartService.Repositories;

/// <inheritdoc />
internal class ProductSearchRepository(EfContext context) : IProductSearchRepository
{
    public async Task<IEnumerable<CartItem>> GetByProductId(Guid productId)
    {
        var items = await context.Set<CartItem>().Where(x => x.ProductId == productId).ToListAsync();
        return items;
    }
}