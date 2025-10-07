using Shop.FavoriteService.Model;

namespace Shop.FavoriteService;

/// <summary>
/// Демо-данные для тестирования
/// </summary>
public static class FakeFavoriteData
{
    /// <summary>
    /// Идентификаторы продуктов
    /// </summary>
    public static List<Guid> ProductIds { get; } =
    [
        Guid.Parse("0891DF1A-ECFB-4E69-BFBE-28FF5A493CC1"),
        Guid.Parse("F267ECEE-337E-4129-8AF0-F3F4337E1B07"),
        Guid.Parse("AAAAAAAA-337E-4129-8AF0-F3F4337E1B07")
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
    /// Списки избранных товаров
    /// </summary>
    public static List<FavoriteList> FavoriteList { get; } =
    [
        new()
        {
            Id = UserIds.Last(),
            Items = new List<FavoriteItem>([
                new FavoriteItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = ProductIds.First()
                }
            ])
        },
        new()
        {
            Id = UserIds.First(),
            Items = new List<FavoriteItem>([
                new FavoriteItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = ProductIds.Skip(1).Take(1).First()
                },
                new FavoriteItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = ProductIds.Last()
                }
            ])
        }
    ];
}