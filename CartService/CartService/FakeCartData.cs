using Shop.CartService.Model;
using Shop.Demo.Data;

namespace CartService;

/// <summary>
/// Демо-данные для тестирования
/// </summary>
public static class FakeCartData
{
    /// <summary>
    /// Идентификаторы продуктов
    /// </summary>
    public static List<Guid> ProductIds => ShopFakeData.Products.Take(10).Select(x => x.Id).ToList();

    /// <summary>
    /// Идентификаторы пользователей
    /// </summary>
    public static List<Guid> UserIds => ShopFakeData.Users.Select(x => x.Id).ToList();

    /// <summary>
    /// Корзины
    /// </summary>
    public static List<Cart> Carts { get; } = ShopFakeData.Carts.Select(x => new Cart
    {
        Id = x.Id,
        CartItems = x.CartItems.Select(z => new CartItem
        {
            Id = Guid.NewGuid(),
            ProductId = z.ProductId,
            Quantity = z.Quantity,
            QuantityInStock = z.QuantityInStock
        }).ToList()

    }).ToList();
}