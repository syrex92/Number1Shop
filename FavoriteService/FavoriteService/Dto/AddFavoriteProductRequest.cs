namespace Shop.FavoriteService.Dto;

/// <summary>
/// Модель запроса на добавление продукта в список избранного
/// </summary>
public class AddFavoriteProductRequest
{
    /// <summary>
    /// Идентификатор продукта
    /// </summary>
    public Guid ProductId { get; set; }
    
}