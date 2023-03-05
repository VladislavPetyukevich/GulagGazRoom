using System.Collections.Concurrent;
using System.Threading.Channels;
using Interview.Domain.Events;

namespace Interview.Backend.Controllers.WebSocket
{
    public class RoomEventDispatcher : IRoomEventDispatcher
    {
        private readonly ConcurrentDictionary<Guid, Channel<IWebSocketEvent>> _queue = new();

        public async Task<IEnumerable<IWebSocketEvent>> ReadAsync(TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource(timeout);

            try
            {
                var list = new List<IWebSocketEvent>();

                foreach (var (_, value) in _queue)
                {
                    list.Add(await value.Reader.ReadAsync(cts.Token));
                }

                return list;
            }
            catch (TaskCanceledException)
            {
                return Enumerable.Empty<IWebSocketEvent>();
            }
        }

        public Task WriteAsync(IWebSocketEvent @event, CancellationToken cancellationToken = default)
        {
            var channel = GetChannel(@event.RoomId);
            return channel.Writer.WriteAsync(@event, cancellationToken).AsTask();
        }

        private static Channel<T> CreateBoundedChannel<T>(int capacity = 100) => Channel.CreateBounded<T>(
            new BoundedChannelOptions(capacity)
            {
                SingleWriter = false,
                SingleReader = false,
                FullMode = BoundedChannelFullMode.DropOldest,
            });

        private Channel<IWebSocketEvent> GetChannel(Guid roomId)
        {
            return _queue.GetOrAdd(roomId, _ => CreateBoundedChannel<IWebSocketEvent>());
        }
    }
}
