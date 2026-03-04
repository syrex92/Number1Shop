namespace NotificationService.Services.RabbitMq;

public sealed class RabbitMqOptions
{
    public string Host { get; init; } = "rabbitmq";
    public int Port { get; init; } = 5672;
    public string User { get; init; } = "guest";
    public string Password { get; init; } = "guest";
    public string VirtualHost { get; init; } = "/";

    public string UiQueue { get; init; } = "shop.notifications.ui";
}

