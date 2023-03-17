namespace Interview.Domain.Events;

public interface IRoomEventDispatcher
{
    Task<IEnumerable<IWebSocketEvent>> ReadAsync(TimeSpan timeSpan);

    Task WriteAsync(IWebSocketEvent @event, CancellationToken cancellationToken = default);

    Task DropEventsAsync(Guid roomId, CancellationToken cancellationToken = default);
}
