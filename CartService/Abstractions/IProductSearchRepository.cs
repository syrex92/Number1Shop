using Shop.CartService.Model;

namespace Shop.CartService.Abstractions;

/// <summary>
/// Репозиторий корзины
/// </summary>
public interface IProductSearchRepository
{
    /// <summary>
    /// Поиск всех корзин, содержащих указанный продукт
    /// </summary>
    /// <param name="productId"></param>
    /// <returns></returns>
    Task<IEnumerable<CartItem>> GetByProductId(Guid productId);
}