using System.Collections.Concurrent;
using System.Threading.Channels;
using Interview.Domain.Events.Events;

namespace Interview.Domain.Events;

public class RoomEventDispatcher : IRoomEventDispatcher
{
    private readonly ConcurrentDictionary<Guid, Channel<IRoomEvent>> _queue = new();
    private readonly SemaphoreSlim _semaphore = new(1);

    public async Task<IEnumerable<IRoomEvent>> ReadAsync(TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);

        try
        {
            var list = new List<IRoomEvent>();

            foreach (var value in _queue.Values)
            {
                var roomEvent = await value.Reader.ReadAsync(cts.Token);
                list.Add(roomEvent);
            }

            return list;
        }
        catch (TaskCanceledException)
        {
            return Enumerable.Empty<IRoomEvent>();
        }
    }

    public async Task WriteAsync(IRoomEvent @event, CancellationToken cancellationToken = default)
    {
        var channel = GetChannel(@event.RoomId);
        await channel.Writer.WriteAsync(@event, cancellationToken).AsTask();
        _semaphore.Release();
    }

    public Task DropEventsAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        if (!_queue.TryRemove(roomId, out var channel))
        {
            return Task.CompletedTask;
        }

        _semaphore.Release();
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

    public Task WaitAsync(CancellationToken cancellationToken = default)
    {
        return _semaphore.WaitAsync(cancellationToken);
    }

    private static Channel<T> CreateBoundedChannel<T>(int capacity = 1024) => Channel.CreateBounded<T>(
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
