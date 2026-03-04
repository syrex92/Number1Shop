using System.Text.Json;

namespace RabbitMqService.Contracts;

public sealed record NotificationEnvelope(
    string Id,
    string Type,
    string Title,
    string Message,
    DateTimeOffset CreatedAt,
    string? UserId = null,
    JsonElement? Data = null
);

