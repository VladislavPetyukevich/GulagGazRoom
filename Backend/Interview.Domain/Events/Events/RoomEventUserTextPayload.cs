namespace Interview.Domain.Events.Events;

public sealed class RoomEventUserTextPayload
{
    public Guid UserId { get; }

    public string Payload { get; }

    public RoomEventUserTextPayload(Guid userId, string payload)
    {
        UserId = userId;
        Payload = payload;
    }
}
