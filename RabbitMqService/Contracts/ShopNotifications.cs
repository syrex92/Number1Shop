namespace RabbitMqService.Contracts;

public static class ShopNotifications
{
    public const string ExchangeName = "shop.notifications";
    public const string ExchangeType = "topic";

    public static class Routing
    {
        public static string Ui(string type) => $"ui.{type}";
    }
}

