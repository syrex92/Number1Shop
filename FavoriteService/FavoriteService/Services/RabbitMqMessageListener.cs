using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shop.FavoriteService.Abstractions;

namespace Shop.FavoriteService.Services;

/// <inheritdoc />
public class RabbitMqMessageListener<T>(
    ILogger<RabbitMqMessageListener<T>> logger,
    IOptions<RabbitMqOptions> options
) : IMessageListener<T>
{
    private ConnectionFactory? _factory;
    private readonly ConcurrentQueue<T> _messages = [];

    /// <inheritdoc />
    public async Task<T[]> GetMessages(CancellationToken cancellationToken)
    {
        if(_factory == null)
            await InitializeListener();
        
        await Task.Delay(1000, cancellationToken);

        var messages = new List<T>();
        while (_messages.TryDequeue(out var message))
        {
            messages.Add(message);
        }
        
        return messages.ToArray();
    }
    
    private async Task InitializeListener()
    {
        try
        {
            _factory = new ConnectionFactory
            {
                HostName = options.Value.Host,
                Port = options.Value.Port,
                Password = options.Value.Password,
                UserName = options.Value.UserName
            };
            
            var connection = await _factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: options.Value.FavoriteProductMessagesQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // await channel.ExchangeDeclarePassiveAsync(
            //     exchange: options.Value.ProductMessagesExchange
            // );
            
            await channel.ExchangeDeclareAsync(
                exchange: options.Value.ProductMessagesExchange,
                type: "fanout",
                durable: true,
                autoDelete: false
            );
            
            await channel.QueueBindAsync(options.Value.FavoriteProductMessagesQueue,
                options.Value.ProductMessagesExchange,
                string.Empty);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += ConsumerOnReceivedAsync;

            await channel.BasicConsumeAsync(queue: options.Value.FavoriteProductMessagesQueue,
                autoAck: true,
                consumer: consumer);
            
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured during RabbitMQ connection: {msg}", e.Message);
            _factory = null;
        }
    }

    
    
    private Task ConsumerOnReceivedAsync(object sender, BasicDeliverEventArgs evt)
    {
        if (evt.Body.Length == 0)
        {
            logger.LogError("Received empty message");
            return Task.CompletedTask;
        }

        try
        {
            var message = JsonSerializer.Deserialize<T>(evt.Body.Span);
            if(message != null)
                _messages.Enqueue(message);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Message could not be deserialized: {msg}", e.Message);
        }
        return Task.CompletedTask;
    }
}