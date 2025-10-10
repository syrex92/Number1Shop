using Shop.CartService.Model;

namespace Shop.CartService;

/// <summary>
/// Демо-данные для тестирования
/// </summary>
public static class FakeCartData
{
    /// <summary>
    /// Идентификаторы продуктов
    /// </summary>
    public static List<Guid> ProductIds { get; } =
    [
        Guid.Parse("0891DF1A-ECFB-4E69-BFBE-28FF5A493CC1"),
        Guid.Parse("F267ECEE-337E-4129-8AF0-F3F4337E1B07")
    ];

    /// <summary>
    /// Идентификаторы пользователей
    /// </summary>
    public static List<Guid> UserIds { get; } =
    [
        Guid.Parse("181BCD21-0EEB-4C9B-A495-F581901A7B1A"),
        Guid.Parse("1F221138-D6A4-47D3-8391-6E1BC9D5B2DE")
    ];

    /// <summary>
    /// Корзины
    /// </summary>
    public static List<Cart> Carts { get; } =
    [
        new()
        {
            Id = UserIds.First(),
            CartItems = []
        },
        new()
        {
            Id = UserIds.Last(),
            CartItems = new List<CartItem>([
                new CartItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = ProductIds.First(),
                    Quantity = 1,
                },
                new CartItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = ProductIds.Last(),
                    Quantity = 2,
                }
            ])
        }
    ];
}