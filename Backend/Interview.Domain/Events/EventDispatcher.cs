using System.Collections.Concurrent;
using System.Threading.Channels;
using Interview.Domain.Events;

namespace Interview.Backend.Controllers.WebSocket
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly ConcurrentDictionary<Guid, Channel<IWebSocketEvent>> _queue = new();

        public async Task<IEnumerable<IWebSocketEvent>> ReadFromRoomsAsync(TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource(timeout);
            try
            {
                var list = new List<IWebSocketEvent>();
                foreach (var (_, value) in _queue)
                {
                    var currentEvent = await value.Reader.ReadAsync(cts.Token);
                    list.Add(currentEvent);
                }

                return list;
            }
            catch (TaskCanceledException ignore)
            {
                return Enumerable.Empty<IWebSocketEvent>();
            }
        }

        public Task<IWebSocketEvent?> ReadAsync(Guid roomId, TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource(timeout);
            try
            {
                var channel = GetChannel(roomId);
                return channel.Reader.ReadAsync(cts.Token).AsTask();
            }
            catch (TaskCanceledException ignore)
            {
                return Task.FromResult<IWebSocketEvent?>(null);
            }
        }

        public Task WriteAsync(IWebSocketEvent @event, CancellationToken cancellationToken = default)
        {
            var channel = GetChannel(@event.RoomId);
            return channel.Writer.WriteAsync(@event, cancellationToken).AsTask();
        }
        
        private Channel<IWebSocketEvent> GetChannel(Guid roomId)
        {
            return _queue.GetOrAdd(roomId, rId => Channel.CreateUnbounded<IWebSocketEvent>(
                new UnboundedChannelOptions
                {
                    SingleReader = false, 
                    SingleWriter = false,
                }));
        }
    }
}
