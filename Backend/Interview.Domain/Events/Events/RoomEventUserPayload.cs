namespace Interview.Domain.Events.Events;

public sealed class RoomEventUserPayload
{
    public Guid UserId { get; }

    public Dictionary<string, object>? AdditionalData { get; }

    public RoomEventUserPayload(Guid userId, Dictionary<string, object>? additionalData = null)
    {
        UserId = userId;
        AdditionalData = additionalData;
    }
}
