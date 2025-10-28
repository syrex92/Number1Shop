namespace Shop.CartService.Abstractions;

/// <summary>
/// Репозиторий корзины
/// </summary>
public interface IRepository<T>
{
    /// <summary>
    /// Получение корзины по идентификатору пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<T?> GetById(Guid userId);
    
    /// <summary>
    /// Добавление новой корзины 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Task<T> Add(T item);
    
    /// <summary>
    /// Изменение корзины
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Task<T> Update(T item);
}