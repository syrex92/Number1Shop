using System.Collections.Concurrent;

namespace NotificationService.Services.Realtime;

public sealed class ConnectedUsersTracker
{
    private readonly ConcurrentDictionary<string, int> _counts = new(StringComparer.Ordinal);

    public void Increment(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) return;
        _counts.AddOrUpdate(userId, 1, (_, current) => current + 1);
    }

    public void Decrement(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) return;

        while (true)
        {
            if (!_counts.TryGetValue(userId, out var current)) return;

            if (current <= 1)
            {
                _counts.TryRemove(userId, out _);
                return;
            }

            if (_counts.TryUpdate(userId, current - 1, current)) return;
        }
    }

    public bool IsConnected(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) return false;
        return _counts.ContainsKey(userId);
    }
}

