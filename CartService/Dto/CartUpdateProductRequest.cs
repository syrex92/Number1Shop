namespace Shop.CartService.Dto;

/// <summary>
/// Модель запроса на обновление корзины
/// </summary>
public class CartUpdateProductRequest
{
    /// <summary>
    /// Идентификатор продукта
    /// </summary>
    public Guid ProductId { get; set; }
    
    /// <summary>
    /// Новое количество. Если значение меньше или равно нулю - продукт удаляется из корзины
    /// </summary>
    public int Quantity { get; set; }
}