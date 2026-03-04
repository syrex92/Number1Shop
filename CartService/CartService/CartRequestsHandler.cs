using System.Security.Claims;
using CartService.Dto;
using Shop.CartService.Abstractions;
using Shop.CartService.Dto;
using Shop.CartService.Model;

namespace CartService;

/// <summary>
/// Обработка запросов к корзине
/// </summary>
public static class CartRequestsHandler
{
    private static async Task<Cart> CreateNewCart(
        Guid userId,
        IRepository<Cart> repository
        )
    {
        var cart = new Cart
        {
            Id = userId,
            CartItems = new List<CartItem>()
        };
        await repository.Add(cart);

        return cart;
    }
    
    /// <summary>
    /// Получение содержимого корзины. Если корзина ещё не существует -
    /// возвращается ответ без товаров, корзина не создаётся
    /// </summary>
    /// <remarks>
    /// Метод получает идентификатор корзины, равный идентификатору пользователя,
    /// из свойств авторизованного пользователя (ClaimType = ClaimTypes.Sid).
    /// </remarks>
    /// <param name="context"></param>
    /// <param name="repository"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> GetCart(
        HttpContext context,
        IRepository<Cart> repository,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var userId = Guid.ParseExact(
                context.User.Claims.Single(x => x.Type == ClaimTypes.Sid).Value,
                "D");

            var cart = await repository.GetById(userId);
            if (cart != null)
                return TypedResults.Ok(new CartResponse
                {
                    Items = cart.CartItems.Select(x => new CartItemResponse
                    {
                        ProductId = x.ProductId,
                        Quantity = x.Quantity
                    }).ToList()
                });

            //await CreateNewCart(userId, repository, cancellationToken);
            
            return TypedResults.Ok(new CartResponse
            {
                Items = []
            });
        }
        catch (Exception e)
        {
            return TypedResults.InternalServerError(e.Message);
        }
    }

    /// <summary>
    /// Добавление товара в корзину. Если корзина пользователя ещё не существует - она создаётся
    /// </summary>
    /// <remarks>
    /// При вызове добавляется количество указанного товара в корзину.
    /// Если новое количество меньше или равно нулю - товар удаляется из корзины
    /// </remarks>
    /// <param name="context"></param>
    /// <param name="repository"></param>
    /// <param name="updateCartRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Описание добавленного элемента <see cref="CartItemResponse"/></returns>
    public static async Task<IResult> AddToCart(
        HttpContext context,
        IRepository<Cart> repository,
        CartUpdateProductRequest updateCartRequest,
        CancellationToken cancellationToken
    )
    {
        if (updateCartRequest.Quantity == 0)
            return TypedResults.BadRequest("Quantity must be greater than zero");

        try
        {
            var userId = Guid.ParseExact(
                context.User.Claims.Single(x => x.Type == ClaimTypes.Sid).Value,
                "D");

            var cart = await repository.GetById(userId) ?? await CreateNewCart(userId, repository);
            
            var item = cart.CartItems.FirstOrDefault(x => x.ProductId == updateCartRequest.ProductId);
            if (item != null)
                return TypedResults.Conflict();

            item = new CartItem
            {
                ProductId = updateCartRequest.ProductId,
                Quantity = updateCartRequest.Quantity
            };

            cart.CartItems.Add(item);

            await repository.Update(cart);

            return TypedResults.Ok(new CartItemResponse()
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            });
        }
        catch (Exception e)
        {
            return TypedResults.InternalServerError(e.Message);
        }
    }

    /// <summary>
    /// Обновление количества товара в корзине 
    /// </summary>
    /// <remarks>
    /// При вызове обновляется количество указанного товара в корзине.
    /// Если новое количество меньше или равно нулю - товар удаляется из корзины
    /// </remarks>
    /// <param name="context"></param>
    /// <param name="repository"></param>
    /// <param name="updateCartRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> UpdateCart(
        HttpContext context,
        IRepository<Cart> repository,
        CartUpdateProductRequest updateCartRequest,
        CancellationToken cancellationToken
    )
    {
        var userId = Guid.ParseExact(
            context.User.Claims.Single(x => x.Type == ClaimTypes.Sid).Value,
            "D");

        var cart = await repository.GetById(userId);
        if (cart == null)
            return TypedResults.NotFound<CartResponse>(null);

        var item = cart.CartItems.FirstOrDefault(x => x.ProductId == updateCartRequest.ProductId);
        if (item == null)
            return TypedResults.NotFound<CartResponse>(null);

        if (updateCartRequest.Quantity > 0)
            item.Quantity = updateCartRequest.Quantity;
        else
            cart.CartItems.Remove(item);

        await repository.Update(cart);


        if (updateCartRequest.Quantity <= 0)
            item.Quantity = 0;

        return TypedResults.Ok(new CartItemResponse()
        {
            ProductId = item.ProductId,
            Quantity = item.Quantity
        });
    }

    /// <summary>
    /// Очистка корзины пользователя
    /// </summary>
    /// <remarks>
    /// При вызове из корзины текущего пользователя удаляются все товары
    /// </remarks>
    /// <param name="context"></param>
    /// <param name="repository"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> ClearCart(
        HttpContext context,
        IRepository<Cart> repository,
        CancellationToken cancellationToken
    )
    {
        var userId = Guid.ParseExact(
            context.User.Claims.Single(x => x.Type == ClaimTypes.Sid).Value,
            "D");

        var cart = await repository.GetById(userId);
        if (cart == null)
            return TypedResults.Ok();

        cart.CartItems = new List<CartItem>();

        await repository.Update(cart);
        return TypedResults.Ok();
    }

    /// <summary>
    /// Проверка доступности сервиса
    /// </summary>
    /// <remarks>
    /// Вызывается для проверки доступности сервиса. Если возвращён статус 200 - значит сервис доступен.
    /// Применяется для контроля состояния сервиса в контейнере
    /// </remarks>
    /// <returns>plain string message</returns>
    public static Task<IResult> CheckHealth()
    {
        return Task.FromResult<IResult>(TypedResults.Text("Cart service working"));
    }
}