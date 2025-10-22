namespace Shop.FavoriteService.Dto;

/// <summary>
/// Модель ответа для отдельного избранного продукта
/// </summary>
public class FavoriteProductResponse
{
    /// <summary>
    /// Идентификатор продукта
    /// </summary>
    public Guid ProductId { get; set; }
    
}

/// <summary>
/// Модель ответа для списка избранных продуктов
/// </summary>
public class FavoriteProductListResponse
{
    /// <summary>
    /// Список избранных товаров
    /// </summary>
    public IEnumerable<FavoriteProductResponse> Items { get; set; } = [];
}