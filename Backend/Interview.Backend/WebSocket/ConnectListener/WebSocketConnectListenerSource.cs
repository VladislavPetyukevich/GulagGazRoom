using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Interview.Backend.WebSocket.ConnectListener;

public class WebSocketConnectListenerSource
{
    private readonly ConcurrentDictionary<Guid, (ImmutableHashSet<Guid> Users, string TwitchChanel)> _queue = new();

    public IReadOnlyDictionary<Guid, (ImmutableHashSet<Guid> Users, string TwitchChanel)> Source => _queue;

    public void Connect(Guid roomId, Guid userId, string twitchChannel)
    {
        _queue.AddOrUpdate(
            roomId,
            _ => (ImmutableHashSet.Create(userId), twitchChannel),
            (_, set) => (set.Users.Add(userId), twitchChannel));
    }

    public void Disconnect(Guid roomId, Guid userId, string twitchChannel)
    {
        _queue.AddOrUpdate(
            roomId,
            s => (ImmutableHashSet<Guid>.Empty, twitchChannel),
            (_, set) => (set.Users.Remove(userId), set.TwitchChanel));
    }
}
