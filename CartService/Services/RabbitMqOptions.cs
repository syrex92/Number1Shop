namespace Shop.CartService.Services;

/// <summary>
/// Настройки соединения с RabbitMQ
/// </summary>
public class RabbitMqOptions
{
    /// <summary>
    /// Название очереди, из которой корзина получает сообщения о товарах
    /// </summary>
    public string CartProductMessagesQueue { get; set; } = string.Empty;
    
    /// <summary>
    /// Название Exchange, к которому необходимо подключить очередь
    /// </summary>
    public string ProductMessagesExchange { get; set; } = string.Empty;
    
    /// <summary>
    /// Хост RabbitMQ 
    /// </summary>
    public string Host { get; set; } = string.Empty;
    
    /// <summary>
    /// Порт подключения к RabbitMQ
    /// </summary>
    public int Port { get; set; } = 5672;
    
    
    /// <summary>
    /// Пароль для подключения к RabbitMQ 
    /// </summary>
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// Имя пользователя для подключения к RabbitMQ
    /// </summary>
    public string UserName { get; set; } = string.Empty;
}