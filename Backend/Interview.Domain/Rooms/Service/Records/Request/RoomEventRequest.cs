using Interview.Domain.Events;
using Interview.Domain.Events.Events;

namespace Interview.Domain.Rooms.Service.Records.Request;

public sealed class RoomEventRequest : IEventRequest
{
    public Guid RoomId { get; }

    public Guid UserId { get; }

    public string Type { get; }

    public Dictionary<string, object>? AdditionalData { get; }

    public RoomEventRequest(Guid roomId, Guid userId, string type, Dictionary<string, object>? additionalData)
    {
        RoomId = roomId;
        UserId = userId;
        Type = type;
        AdditionalData = additionalData;
    }

    public IRoomEvent ToRoomEvent() => new RoomEvent<RoomEventUserPayload>(RoomId, Type, new RoomEventUserPayload(UserId, AdditionalData));
}
