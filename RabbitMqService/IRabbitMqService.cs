using System;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMqService
{
    public interface IRabbitMqService : IAsyncDisposable, IDisposable
    {
        Task PublishMessageAsync(string exchangeName, string routingKey, object message, CancellationToken cancellationToken = default);
        Task SubscribeToQueueAsync(string queueName, string exchangeName, Func<string, Task> onMessageReceived, CancellationToken cancellationToken = default);

        void PublishMessage(string exchangeName, string routingKey, object message);
        void SubscribeToQueue(string queueName, string exchangeName, Action<string> onMessageReceived);
    }
}
