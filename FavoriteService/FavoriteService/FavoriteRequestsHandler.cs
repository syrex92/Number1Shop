using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Shop.FavoriteService.Abstractions;
using Shop.FavoriteService.Dto;
using Shop.FavoriteService.Model;

namespace Shop.FavoriteService;

/// <summary>
/// Обработка запросов к списку избранных продуктов
/// </summary>
public static class FavoriteRequestsHandler
{
    /// <summary>
    /// Получение содержимого списка избранных
    /// </summary>
    /// <remarks>
    /// Метод получает идентификатор списка, равный идентификатору пользователя,
    /// из свойств авторизованного пользователя (ClaimType = ClaimTypes.Sid).
    /// </remarks>
    /// <param name="context"></param>
    /// <param name="repository"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> GetFavoriteProducts(
        HttpContext context,
        IRepository<FavoriteList> repository,
        CancellationToken cancellationToken
    )
    {
        var userId = Guid.ParseExact(
            context.User.Claims.Single(x => x.Type == ClaimTypes.Sid).Value, 
            "D");

        var favoriteList = await repository.GetById(userId);
        if (favoriteList != null)
            return TypedResults.Ok(new FavoriteProductListResponse
            {
                Items = (favoriteList.Items?.Select(x => new FavoriteProductResponse()
                {
                    ProductId = x.ProductId
                }) ?? []).ToList()
            });


        favoriteList = new FavoriteList
        {
            Id = userId
        };
        await repository.Add(favoriteList);

        return TypedResults.Ok(new FavoriteProductListResponse
        {
            Items = []
        });
    }

    /// <summary>
    /// Добавление продукта в список избранных 
    /// </summary>
    /// <remarks>
    /// При вызове указанный товар добавляется в список избранных для текущего пользователя.
    /// </remarks>
    /// <param name="context"></param>
    /// <param name="listRepository"></param>
    /// <param name="itemRepository"></param>
    /// <param name="addProductRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> AddFavoriteProduct(
        HttpContext context,
        IRepository<FavoriteList> listRepository,
        IRepository<FavoriteItem> itemRepository,
        AddFavoriteProductRequest addProductRequest,
        CancellationToken cancellationToken
    )
    {
        
        var userId = Guid.ParseExact(
            context.User.Claims.Single(x => x.Type == ClaimTypes.Sid).Value, 
            "D");
         
        var favoriteList = await listRepository.GetById(userId);
        if (favoriteList == null)
        {
            favoriteList = new FavoriteList
            {
                Id = userId
            };
            await listRepository.Add(favoriteList);
        }
        
        var favoriteProduct = favoriteList.Items?
            .FirstOrDefault(x => x.ProductId == addProductRequest.ProductId);

        if (favoriteProduct == null)
        {
            // add new
            var item = new FavoriteItem
            {
                Id = Guid.NewGuid(),
                ProductId = addProductRequest.ProductId
            };
            
            await itemRepository.Add(item);
            
            favoriteList.Items?.Add(item);
            
            await listRepository.Update(favoriteList);
        }

        return TypedResults.Created("/", new FavoriteProductListResponse
        {
            Items = favoriteList.Items?.Select(x => new FavoriteProductResponse
            {
                ProductId = x.ProductId
            }).ToList() ?? []
        });
    }

    /// <summary>
    /// Очистка списка избранных продуктов пользователя
    /// </summary>
    /// <remarks>
    /// При вызове из списка избранных продуктов текущего пользователя удаляются все товары
    /// </remarks>
    /// <param name="context"></param>
    /// <param name="repository"></param>
    /// <param name="productId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> RemoveFavoriteProduct(
        HttpContext context,
        IRepository<FavoriteList> repository,
        [FromRoute] Guid productId,
        CancellationToken cancellationToken
    )
    {
        var userId = Guid.ParseExact(
            context.User.Claims.Single(x => x.Type == ClaimTypes.Sid).Value, 
            "D");

        var favoriteList = await repository.GetById(userId);
        if (favoriteList == null)
            return TypedResults.Ok();

        var product = favoriteList.Items?.FirstOrDefault(x => x.ProductId == productId);
        if(product == null)
            return TypedResults.NotFound();
        
        favoriteList.Items?.Remove(product);

        await repository.Update(favoriteList);
        return TypedResults.Ok();
    }
    
    
    /// <summary>
    /// Очистка списка избранных продуктов
    /// </summary>
    /// <remarks>
    /// При вызове из списка избранных продуктов текущего пользователя удаляются все элементы
    /// </remarks>
    /// <param name="context"></param>
    /// <param name="repository"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> ClearFavoriteProducts(
        HttpContext context,
        IRepository<FavoriteList> repository,
        CancellationToken cancellationToken
    )
    {
        var userId = Guid.ParseExact(
            context.User.Claims.Single(x => x.Type == ClaimTypes.Sid).Value, 
            "D");

        var favoriteList = await repository.GetById(userId);
        if (favoriteList == null)
            return TypedResults.Ok(); // nothing to clear

        favoriteList.Items = new List<FavoriteItem>();

        await repository.Update(favoriteList);
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
        return Task.FromResult<IResult>(TypedResults.Text("FavoriteList service working"));
    }

    
}