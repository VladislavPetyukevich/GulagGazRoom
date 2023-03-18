using Interview.Domain.Events.Events;

namespace Interview.Domain.Events;

public interface IRoomEventDispatcher
{
    Task<IEnumerable<IRoomEvent>> ReadAsync(TimeSpan timeSpan);

    Task WriteAsync(IRoomEvent @event, CancellationToken cancellationToken = default);

    Task DropEventsAsync(Guid roomId, CancellationToken cancellationToken = default);
}
