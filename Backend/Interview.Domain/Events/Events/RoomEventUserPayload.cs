namespace Interview.Domain.Events.Events;

public sealed class RoomEventUserPayload
{
    public Guid UserId { get; }

    public RoomEventUserPayload(Guid userId)
    {
        UserId = userId;
    }
}
