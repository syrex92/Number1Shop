using Shop.FavoriteService.Abstractions;

namespace Shop.FavoriteService.Model;

/// <summary>
/// Список избранных продуктов
/// </summary>
public class FavoriteList : BaseEntity
{
    
    /// <summary>
    /// Список продуктов
    /// </summary>
    public ICollection<FavoriteItem>? Items { get; set; }
}