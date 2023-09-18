using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Interview.Backend.WebSocket.Events.ConnectionListener;

public class VideoChatConnectionListener : IConnectionListener, IVideChatConnectionProvider
{
    private ConcurrentDictionary<Guid, ImmutableList<Payload>> _store = new();

    public Task OnConnectAsync(WebSocketConnectDetail detail, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task OnDisconnectAsync(WebSocketConnectDetail detail, CancellationToken cancellationToken)
    {
        _store.AddOrUpdate(
            detail.Room.Id,
            _ => ImmutableList<Payload>.Empty,
            (_, list) => list.Remove(new Payload(detail.User.Id, detail.WebSocket), EqualityComparer.Instance));
        return Task.CompletedTask;
    }

    public void Connect(Guid roomId, Guid userId, System.Net.WebSockets.WebSocket webSocket)
    {
        _store.AddOrUpdate(
            roomId,
            _ => ImmutableList.Create(new Payload(userId, webSocket)),
            (_, list) => list.Add(new Payload(userId, webSocket)));
    }

    public bool TryGetConnections(Guid roomId, [NotNullWhen(true)] out IReadOnlyCollection<(Guid UserId, System.Net.WebSockets.WebSocket WebSocket)>? connections)
    {
        if (!_store.TryGetValue(roomId, out var subscriber) || subscriber.Count == 0)
        {
            connections = null;
            return false;
        }

        connections = subscriber.Select(e => (e.UserId, e.Connection)).ToList();
        return true;
    }

    private record Payload(Guid UserId, System.Net.WebSockets.WebSocket Connection);

    private sealed class EqualityComparer : IEqualityComparer<Payload>
    {
        public static readonly IEqualityComparer<Payload> Instance = new EqualityComparer();

        public bool Equals(Payload? x, Payload? y)
        {
            return x is not null &&
                   y is not null &&
                   ReferenceEquals(x.Connection, y.Connection);
        }

        public int GetHashCode(Payload obj) => obj.Connection.GetHashCode();
    }
}

public interface IVideChatConnectionProvider
{
    void Connect(Guid roomId, Guid userId, System.Net.WebSockets.WebSocket webSocket);

    bool TryGetConnections(
        Guid roomId,
        [NotNullWhen(true)] out IReadOnlyCollection<(Guid UserId, System.Net.WebSockets.WebSocket WebSocket)>? connections);
}
