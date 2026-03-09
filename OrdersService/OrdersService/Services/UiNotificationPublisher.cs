using RabbitMqService;
using RabbitMqService.Contracts;

namespace OrdersService.Services;

public interface IUiNotificationPublisher
{
    Task PublishAsync(string userId, string type, string title, string message, object? data = null, CancellationToken cancellationToken = default);
}

public sealed class UiNotificationPublisher : IUiNotificationPublisher
{
    private readonly RabbitMqClientOptions _options;

    public UiNotificationPublisher(RabbitMqClientOptions options)
    {
        _options = options;
    }

    public async Task PublishAsync(string userId, string type, string title, string message, object? data = null, CancellationToken cancellationToken = default)
    {
        var envelope = new NotificationEnvelope(
            Id: Guid.NewGuid().ToString("N"),
            Type: type,
            Title: title,
            Message: message,
            CreatedAt: DateTimeOffset.UtcNow,
            UserId: userId,
            Data: data == null ? null : System.Text.Json.JsonSerializer.SerializeToElement(data)
        );

        try
        {
            await using IRabbitMqService rabbit = new RabbitMqService.RabbitMqService(_options);
            await rabbit.PublishMessageAsync(
                exchangeName: ShopNotifications.ExchangeName,
                exchangeType: ShopNotifications.ExchangeType,
                routingKey: ShopNotifications.Routing.Ui(type),
                message: envelope,
                cancellationToken: cancellationToken);
        }
        catch
        {
            // Notifications must not break core flow
        }
    }
}

