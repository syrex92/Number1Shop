using Shop.FavoriteService.Abstractions;

namespace Shop.FavoriteService.Model;

/// <summary>
/// Сущность элемента списка избранных товаров
/// </summary>
public class FavoriteItem : BaseEntity
{
    /// <summary>
    /// Идентификатор продукта
    /// </summary>
    public Guid ProductId { get; set; }
   
}