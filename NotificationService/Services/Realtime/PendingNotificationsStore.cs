using System.Collections.Concurrent;
using NotificationService.Models;

namespace NotificationService.Services.Realtime;

public sealed class PendingNotificationsStore
{
    private const int MaxPerUser = 50;

    private readonly ConcurrentDictionary<string, ConcurrentQueue<NotificationEnvelope>> _pending =
        new(StringComparer.Ordinal);

    public void Enqueue(string userId, NotificationEnvelope envelope)
    {
        if (string.IsNullOrWhiteSpace(userId)) return;

        var queue = _pending.GetOrAdd(userId, _ => new ConcurrentQueue<NotificationEnvelope>());
        queue.Enqueue(envelope);

        // Bound memory per user (best-effort).
        while (queue.Count > MaxPerUser && queue.TryDequeue(out _))
        {
        }
    }

    public IReadOnlyList<NotificationEnvelope> Drain(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) return Array.Empty<NotificationEnvelope>();

        if (!_pending.TryRemove(userId, out var queue)) return Array.Empty<NotificationEnvelope>();

        var list = new List<NotificationEnvelope>();
        while (queue.TryDequeue(out var item))
        {
            list.Add(item);
        }

        return list;
    }
}

