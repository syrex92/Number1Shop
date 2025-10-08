namespace Shop.CartService.Dto;

/// <summary>
/// Модель ответа для элемента корзины
/// </summary>
public class CartItemResponse
{
    /// <summary>
    /// Идентификатор продукта
    /// </summary>
    public Guid ProductId { get; set; }
    
    /// <summary>
    /// Количество в корзине
    /// </summary>
    public int Quantity { get; set; }
}

/// <summary>
/// Модель ответа для корзины
/// </summary>
public class CartResponse
{
    /// <summary>
    /// Список товаров в корзине
    /// </summary>
    public IEnumerable<CartItemResponse> Items { get; set; } = [];
}