using CatalogService.Configurations;
using MassTransit;

namespace CatalogService.Helpers
{
    internal static class RabbitConfigurator
    {
        /// <summary>
        /// Конфигурация rmq
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configuration"></param>
        /// <exception cref="InvalidProgramException"></exception>
        internal static void ConfigureRmq(IRabbitMqBusFactoryConfigurator configurator, IConfiguration configuration)
        {
            var host = configuration["RMQ_HOST"];
            var port = int.TryParse(configuration["RMQ_PORT"], out int parsedPort) ? parsedPort : 0;
            var userName = configuration["RMQ_USER"];
            var password = configuration["RMQ_PASSWORD"];
            var vHost = "/";

            var rmqSettings = configuration.GetSection("RabbitMqConfiguration").Get<RabbitMqConfiguration>();

            if (rmqSettings == null && host == null)
            {
                throw new InvalidProgramException("Configudation for rabbit is empty");
            }

            configurator.Host(host ?? rmqSettings?.Host,
                vHost ?? rmqSettings?.VHost,
                h =>
                {
                    h.Username(userName ?? rmqSettings.Login);
                    h.Password(password ?? rmqSettings.Password);
                });
        }

        /// <summary>
        /// Регистрация консьюмеров
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context"></param>
        internal static void RegisterEndPoints(IRabbitMqBusFactoryConfigurator configurator, IBusRegistrationContext context)
        {
            // Example
            configurator.ReceiveEndpoint($"queueName", e =>
            {
                // e.ConfigureConsumer<IConsumer>(context);
                e.UseMessageRetry(r =>
                {
                    r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
                });
            });
        }
    }
}
