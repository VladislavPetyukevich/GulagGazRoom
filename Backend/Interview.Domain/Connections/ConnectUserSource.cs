using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Interview.Domain.Connections;

public class ConnectUserSource : IConnectUserSource
{
    private readonly ConcurrentDictionary<Guid, (ImmutableHashSet<Guid> Users, string TwitchChanel)> _queue = new();
    private readonly SemaphoreSlim _semaphore = new(1);

    public ICollection<Guid> ActiveRooms => _queue.Keys;

    public IReadOnlyDictionary<Guid, (ImmutableHashSet<Guid> Users, string TwitchChanel)> Source => _queue;

    public void Connect(Guid roomId, Guid userId, string twitchChannel)
    {
        _queue.AddOrUpdate(
            roomId,
            _ => (ImmutableHashSet.Create(userId), twitchChannel),
            (_, set) => (set.Users.Add(userId), twitchChannel));
        _semaphore.Release();
    }

    public void Disconnect(Guid roomId, Guid userId, string twitchChannel)
    {
        _queue.AddOrUpdate(
            roomId,
            s => (ImmutableHashSet<Guid>.Empty, twitchChannel),
            (_, set) => (set.Users.Remove(userId), set.TwitchChanel));
        _semaphore.Release();
    }

    public Task WaitAsync(CancellationToken cancellationToken = default)
    {
        return _semaphore.WaitAsync(cancellationToken);
    }
}
