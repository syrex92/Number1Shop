namespace Shop.FavoriteService.Abstractions;

/// <summary>
/// Интерфейс объекта, полслушивающего очередь сообщений
/// </summary>
public interface IMessageListener<T>
{
    /// <summary>
    /// Получение списка сообщений
    /// </summary>
    /// <returns>Список сообщений</returns>
    Task<T[]> GetMessages(CancellationToken cancellationToken);
}