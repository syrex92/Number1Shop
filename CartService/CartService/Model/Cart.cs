using Shop.CartService.Abstractions;

namespace Shop.CartService.Model;

/// <summary>
/// Корзина пользователя
/// </summary>
public class Cart : BaseEntity
{
    
    /// <summary>
    /// Список товаров в корзине
    /// </summary>
    public ICollection<CartItem>? CartItems { get; set; }
}