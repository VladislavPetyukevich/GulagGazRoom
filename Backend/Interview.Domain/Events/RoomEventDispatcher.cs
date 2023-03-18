using System.Collections.Concurrent;
using System.Threading.Channels;
using Interview.Domain.Events.Events;

namespace Interview.Domain.Events;

public class RoomEventDispatcher : IRoomEventDispatcher
{
    private readonly ConcurrentDictionary<Guid, Channel<IRoomEvent>> _queue = new();

    public async Task<IEnumerable<IRoomEvent>> ReadAsync(TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);

        try
        {
            var list = new List<IRoomEvent>();

            foreach (var (_, value) in _queue)
            {
                list.Add(await value.Reader.ReadAsync(cts.Token));
            }

            return list;
        }
        catch (TaskCanceledException)
        {
            return Enumerable.Empty<IRoomEvent>();
        }
    }

    public Task WriteAsync(IRoomEvent @event, CancellationToken cancellationToken = default)
    {
        var channel = GetChannel(@event.RoomId);
        return channel.Writer.WriteAsync(@event, cancellationToken).AsTask();
    }

    public Task DropEventsAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        if (!_queue.TryRemove(roomId, out var channel))
        {
            return Task.CompletedTask;
        }

        try
        {
            channel.Writer.TryComplete();
        }
        catch
        {
            // ignore
        }

        return Task.CompletedTask;
    }

    private static Channel<T> CreateBoundedChannel<T>(int capacity = 100) => Channel.CreateBounded<T>(
        new BoundedChannelOptions(capacity)
        {
            SingleWriter = false,
            SingleReader = false,
            FullMode = BoundedChannelFullMode.DropOldest,
        });

    private Channel<IRoomEvent> GetChannel(Guid roomId)
    {
        return _queue.GetOrAdd(roomId, _ => CreateBoundedChannel<IRoomEvent>());
    }
}
