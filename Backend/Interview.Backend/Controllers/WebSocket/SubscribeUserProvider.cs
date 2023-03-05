using System.Collections.Concurrent;

namespace Interview.Backend.Controllers.WebSocket
{
    public class SubscribeUserProvider
    {
        private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<System.Net.WebSockets.WebSocket, TaskCompletionSource<object>>> _users = new();

        public Task SubscribeAsync(Guid roomId, System.Net.WebSockets.WebSocket webSocket)
        {
            var users = _users.GetOrAdd(roomId,
                _ => new ConcurrentDictionary<System.Net.WebSockets.WebSocket, TaskCompletionSource<object>>());

            var cst = new TaskCompletionSource<object>();
            users.TryAdd(webSocket, cst);
            return cst.Task;
        }

        public bool TryGetUsers(Guid roomId, out ConcurrentDictionary<System.Net.WebSockets.WebSocket, TaskCompletionSource<object>> users)
        {
            return _users.TryGetValue(roomId, out users);
        }
    }
}
