using System.Collections;
using System.Collections.Concurrent;

namespace Interview.Backend.WebSocket.UserByRoom
{
    public sealed class UserByRoomSubscriberCollection : IEnumerable<UserSubscriber>
    {
        private readonly ConcurrentDictionary<System.Net.WebSockets.WebSocket, TaskCompletionSource<object>> _users = new();

        public async Task AddAsync(System.Net.WebSockets.WebSocket webSocket, CancellationToken cancellationToken = default)
        {
            var cst = new TaskCompletionSource<object>();
            _users.TryAdd(webSocket, cst);
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
                tcs.SetCanceled(stoppingToken);
            }
            catch
            {
                // ignore
            }
        }

        public IEnumerator<UserSubscriber> GetEnumerator() =>
            _users.Keys.Select(e => new UserSubscriber(e)).ToList().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
