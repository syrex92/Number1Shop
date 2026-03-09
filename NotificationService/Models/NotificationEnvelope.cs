using System.Text.Json;

namespace NotificationService.Models;

public sealed record NotificationEnvelope(
    string Id,
    string Type,
    string Title,
    string Message,
    DateTimeOffset CreatedAt,
    string? UserId = null,
    string? Email = null,
    JsonElement? Data = null
);

