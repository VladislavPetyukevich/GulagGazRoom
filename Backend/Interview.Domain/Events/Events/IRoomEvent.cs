namespace Interview.Domain.Events.Events;

public interface IRoomEvent
{
    Guid RoomId { get; }

    string Type { get; }

    bool Stateful { get; }

    string? BuildStringPayload();
}

public interface IRoomEvent<out T> : IRoomEvent
{
    T? Value { get; }
}
