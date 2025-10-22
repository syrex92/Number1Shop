using Shop.CartService.Abstractions;

namespace Shop.CartService.Model;

/// <summary>
/// Сущность элемента корзины
/// </summary>
public class CartItem : BaseEntity
{
    /// <summary>
    /// Идентификатор продукта
    /// </summary>
    public Guid ProductId { get; set; }
    
    /// <summary>
    /// Количество в корзине
    /// </summary>
    public int Quantity { get; set; }
    
    /// <summary>
    /// Количество на складе
    /// </summary>
    public int QuantityInStock { get; set; }
}