using Shop.FavoriteService.Model;

namespace Shop.FavoriteService.Abstractions;

/// <summary>
/// Репозиторий избранного
/// </summary>
public interface IProductSearchRepository
{
    /// <summary>
    /// Поиск всех списоков избранного, содержащих указанный продукт
    /// </summary>
    /// <param name="productId"></param>
    /// <returns></returns>
    Task<IEnumerable<FavoriteItem>> GetByProductId(Guid productId);
}