using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Interview.Backend.WebSocket.UserByRoom;

public class UserByRoomEventSubscriber
{
    private readonly ConcurrentDictionary<Guid, UserByRoomSubscriberCollection> _users = new();

    public Task SubscribeAsync(Guid roomId, System.Net.WebSockets.WebSocket webSocket, Guid userId, CancellationToken cancellationToken = default)
    {
        var webSocketPipe = _users.GetOrAdd(roomId, _ => new UserByRoomSubscriberCollection());
        return webSocketPipe.AddAsync(webSocket, userId, cancellationToken);
    }

    public bool TryGetSubscribers(Guid roomId, [NotNullWhen(true)] out UserByRoomSubscriberCollection? users)
    {
        return _users.TryGetValue(roomId, out users);
    }
}
