using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using NotificationService.Hubs;
using NotificationService.Models;
using RabbitMqService;

namespace NotificationService.Services.RabbitMq;

public sealed class NotificationConsumerHostedService : BackgroundService
{
    private readonly RabbitMqOptions _options;
    private readonly IHubContext<NotificationsHub> _hub;
    private readonly RabbitMqClientOptions _clientOptions;
    private readonly ILogger<NotificationConsumerHostedService> _logger;

    public NotificationConsumerHostedService(
        IOptions<RabbitMqOptions> options,
        IHubContext<NotificationsHub> hub,
        RabbitMqClientOptions clientOptions,
        ILogger<NotificationConsumerHostedService> logger)
    {
        _options = options.Value;
        _hub = hub;
        _clientOptions = clientOptions;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using IRabbitMqService rabbit = new RabbitMqService.RabbitMqService(_clientOptions);

                await rabbit.SubscribeToQueueAsync(
                    queueName: _options.UiQueue,
                    exchangeName: "shop.notifications",
                    exchangeType: "topic",
                    bindingKey: "ui.#",
                    onMessageReceived: HandleUiMessageAsync,
                    cancellationToken: stoppingToken);

                _logger.LogInformation("NotificationService consumers started. UI={UiQueue}", _options.UiQueue);

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RabbitMQ connection failed. Retrying in 5s");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task HandleUiMessageAsync(string json)
    {
        var envelope = Deserialize(json);
        if (envelope == null)
        {
            _logger.LogWarning("UI notification: failed to deserialize message: {Json}", json);
            return;
        }

        if (!string.IsNullOrWhiteSpace(envelope.UserId))
        {
            await _hub.Clients.Group($"user:{envelope.UserId}").SendAsync("notification", envelope);
        }
        else
        {
            await _hub.Clients.All.SendAsync("notification", envelope);
        }
    }

    private static NotificationEnvelope? Deserialize(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<NotificationEnvelope>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
        catch
        {
            return null;
        }
    }
}

