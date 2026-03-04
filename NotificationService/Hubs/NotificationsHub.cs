using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Services.Realtime;

namespace NotificationService.Hubs;

public sealed class NotificationsHub : Hub
{
    private readonly ConnectedUsersTracker _connected;
    private readonly PendingNotificationsStore _pending;

    public NotificationsHub(ConnectedUsersTracker connected, PendingNotificationsStore pending)
    {
        _connected = connected;
        _pending = pending;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.Sid)?.Value
                     ?? Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrWhiteSpace(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");
            _connected.Increment(userId);

            var backlog = _pending.Drain(userId);
            foreach (var item in backlog)
            {
                await Clients.Caller.SendAsync("notification", item);
            }
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.Sid)?.Value
                     ?? Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrWhiteSpace(userId))
        {
            _connected.Decrement(userId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}

