using Interview.Domain.Events.Events;
using Interview.Domain.Events.Storage;

namespace Interview.Domain.Events.Sender;

public class StoreEventSenderAdapter : IEventSenderAdapter
{
    private readonly IEventSenderAdapter _root;
    private readonly IEventStorage _eventStorage;

    public StoreEventSenderAdapter(IEventSenderAdapter root, IEventStorage eventStorage)
    {
        _root = root;
        _eventStorage = eventStorage;
    }

    public async Task SendAsync<T>(T @event, IEventSender<T> sender, CancellationToken cancellationToken)
        where T : IRoomEvent
    {
        await _root.SendAsync(@event, sender, cancellationToken);

        var storageEvent = new StorageEvent
        {
            Id = @event.Id,
            RoomId = @event.RoomId,
            Type = @event.Type,
            Payload = @event.BuildStringPayload(),
            Stateful = @event.Stateful,
            CreatedAt = @event.CreatedAt,
        };

        await _eventStorage.AddAsync(storageEvent, cancellationToken);
    }
}
