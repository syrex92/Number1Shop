namespace CartService.Dto;

/// <summary>
/// Модель запроса на обновление корзины
/// </summary>
public class CartUpdateProductRequest
{
    /// <summary>
    /// Идентификатор продукта
    /// </summary>
    public required Guid ProductId { get; set; }
    
    /// <summary>
    /// Новое количество, должно быть больше нуля.
    /// </summary>
    public required int Quantity { get; set; }
}