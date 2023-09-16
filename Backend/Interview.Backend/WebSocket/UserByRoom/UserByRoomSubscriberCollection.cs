using System.Collections;
using System.Collections.Concurrent;

namespace Interview.Backend.WebSocket.UserByRoom;

public sealed class UserByRoomSubscriberCollection : IEnumerable<UserSubscriber>
{
    private readonly ConcurrentDictionary<System.Net.WebSockets.WebSocket, StorePayload> _users = new();

    public async Task AddAsync(System.Net.WebSockets.WebSocket webSocket, Guid userId, CancellationToken cancellationToken = default)
    {
        var cst = new TaskCompletionSource<object>();
        _users.TryAdd(webSocket, new StorePayload(cst, userId));
        await using (cancellationToken.Register(() => cst.TrySetCanceled()))
        {
            await cst.Task;
        }
    }

    public void Remove(UserSubscriber subscriber, CancellationToken stoppingToken)
    {
        if (!_users.TryRemove(subscriber.WebSocket, out var tcs))
        {
            return;
        }

        try
        {
            tcs.CompletionSource.SetCanceled(stoppingToken);
        }
        catch
        {
            // ignore
        }
    }

    public IEnumerator<UserSubscriber> GetEnumerator() =>
        _users.Select(e => new UserSubscriber(e.Key, e.Value.UserId)).ToList().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private record StorePayload(TaskCompletionSource<object> CompletionSource, Guid UserId);
}
