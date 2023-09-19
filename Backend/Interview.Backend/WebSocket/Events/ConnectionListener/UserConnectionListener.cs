using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Interview.Backend.WebSocket.Events.ConnectionListener;

public class UserConnectionListener : IConnectionListener, IUserWebSocketConnectionProvider
{
    private readonly ConcurrentDictionary<Guid, ImmutableList<WebSocketConnectDetail>> _store = new();

    public Task OnConnectAsync(WebSocketConnectDetail detail, CancellationToken cancellationToken)
    {
        _store.AddOrUpdate(
            detail.User.Id,
            _ => ImmutableList.Create(detail),
            (_, list) => list.Add(detail));
        return Task.CompletedTask;
    }

    public Task OnDisconnectAsync(WebSocketConnectDetail detail, CancellationToken cancellationToken)
    {
        _store.AddOrUpdate(
            detail.User.Id,
            _ => ImmutableList<WebSocketConnectDetail>.Empty,
            (_, list) => list.Remove(detail));
        return Task.CompletedTask;
    }

    public bool TryGetConnections(Guid userId, [NotNullWhen(true)] out IReadOnlyCollection<System.Net.WebSockets.WebSocket>? connections)
    {
        if (_store.TryGetValue(userId, out var details))
        {
            connections = details.Select(e => e.WebSocket).ToList();
            return true;
        }

        connections = null;
        return false;
    }
}

public interface IUserWebSocketConnectionProvider
{
    bool TryGetConnections(
        Guid userId,
        [NotNullWhen(true)] out IReadOnlyCollection<System.Net.WebSockets.WebSocket>? connections);
}
