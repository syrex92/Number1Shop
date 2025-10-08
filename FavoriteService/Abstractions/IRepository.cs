namespace Shop.FavoriteService.Abstractions;

/// <summary>
/// Репозиторий
/// </summary>
public interface IRepository<T>
{
    /// <summary>
    /// Получение элемента по идентификатору пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<T?> GetById(Guid userId);
    
    /// <summary>
    /// Добавление нового элемента 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Task<T> Add(T item);
    
    /// <summary>
    /// Изменение элемента
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    Task<T> Update(T item);
}