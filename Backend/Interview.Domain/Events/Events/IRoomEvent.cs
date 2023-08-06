namespace Interview.Domain.Events.Events;

public interface IRoomEvent
{
    Guid RoomId { get; }

    string Type { get; }
}

public interface IRoomEvent<out T> : IRoomEvent
{
    T? Value { get; }
}
