using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;

namespace RabbitMqService
{
    public class RabbitMqService : IRabbitMqService, IAsyncDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private bool _disposed;
        
        public RabbitMqService() : this(new RabbitMqClientOptions())
        {
        }

        public RabbitMqService(RabbitMqClientOptions options)
        {
            var factory = new ConnectionFactory
            {
                HostName = options.HostName,
                UserName = options.UserName,
                Password = options.Password,
                Port = options.Port,
                VirtualHost = options.VirtualHost
            };

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
            _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: options.PrefetchCount, global: false).GetAwaiter().GetResult();
        }

        public async Task PublishMessageAsync(string exchangeName, string routingKey, object message, CancellationToken cancellationToken = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(RabbitMqService));

            await EnsureExchangeExistsAsync(exchangeName, ExchangeType.Fanout, cancellationToken);

            var jsonMessage = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            await _channel.BasicPublishAsync(
                exchange: exchangeName,
                routingKey: routingKey,
                body: body,
                cancellationToken: cancellationToken
            );
        }

        public void PublishMessage(string exchangeName, string routingKey, object message)
        {
            PublishMessageAsync(exchangeName, routingKey, message).GetAwaiter().GetResult();
        }

        public async Task SubscribeToQueueAsync(string queueName, string exchangeName, Func<string, Task> onMessageReceived, CancellationToken cancellationToken = default)
        {
            await EnsureExchangeExistsAsync(exchangeName, ExchangeType.Fanout, cancellationToken);
            await EnsureQueueExistsAsync(queueName, exchangeName, "", cancellationToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    await onMessageReceived(message);
                    await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken);
                }
                catch
                {
                    await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, cancellationToken);
                }
            };

            await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: cancellationToken);
        }

        public void SubscribeToQueue(string queueName, string exchangeName, Action<string> onMessageReceived)
        {
            SubscribeToQueueAsync(queueName, exchangeName, message =>
            {
                onMessageReceived(message);
                return Task.CompletedTask;
            }).GetAwaiter().GetResult();
        }

        public async Task PublishMessageAsync(string exchangeName, string exchangeType, string routingKey, object message, CancellationToken cancellationToken = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(RabbitMqService));

            await EnsureExchangeExistsAsync(exchangeName, exchangeType, cancellationToken);

            var jsonMessage = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            await _channel.BasicPublishAsync(
                exchange: exchangeName,
                routingKey: routingKey,
                body: body,
                cancellationToken: cancellationToken
            );
        }

        public void PublishMessage(string exchangeName, string exchangeType, string routingKey, object message)
        {
            PublishMessageAsync(exchangeName, exchangeType, routingKey, message).GetAwaiter().GetResult();
        }

        public async Task SubscribeToQueueAsync(string queueName, string exchangeName, string exchangeType, string bindingKey, Func<string, Task> onMessageReceived, CancellationToken cancellationToken = default)
        {
            await EnsureExchangeExistsAsync(exchangeName, exchangeType, cancellationToken);
            await EnsureQueueExistsAsync(queueName, exchangeName, bindingKey, cancellationToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    await onMessageReceived(message);
                    await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken);
                }
                catch
                {
                    await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, cancellationToken);
                }
            };

            await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: cancellationToken);
        }

        public void SubscribeToQueue(string queueName, string exchangeName, string exchangeType, string bindingKey, Action<string> onMessageReceived)
        {
            SubscribeToQueueAsync(queueName, exchangeName, exchangeType, bindingKey, message =>
            {
                onMessageReceived(message);
                return Task.CompletedTask;
            }).GetAwaiter().GetResult();
        }

        private async Task EnsureQueueExistsAsync(string queueName, string exchangeName, string bindingKey, CancellationToken cancellationToken = default)
        {
            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken
            );

            await _channel.QueueBindAsync(
                queue: queueName,
                exchange: exchangeName,
                routingKey: bindingKey,
                arguments: null,
                cancellationToken: cancellationToken
            );
        }

        private async Task EnsureExchangeExistsAsync(string exchangeName, string exchangeType, CancellationToken cancellationToken = default)
        {
            await _channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: exchangeType,
                durable: true,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken
            );
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _disposed = true;

            if (_channel != null)
            {
                await _channel.CloseAsync();
                await _channel.DisposeAsync();
            }

            if (_connection != null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }
        }

        public void Dispose()
        {
            DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
    }
}
