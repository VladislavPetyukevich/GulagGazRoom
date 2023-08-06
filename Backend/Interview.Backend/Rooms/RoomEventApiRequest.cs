using Interview.Domain.Rooms.Service.Records;
using Interview.Domain.Rooms.Service.Records.Request;

namespace Interview.Backend.Rooms;

public sealed class RoomEventApiRequest
{
    public Guid RoomId { get; set; }

    public string Type { get; set; }

    public Dictionary<string, object>? AdditionalData { get; set; }

    public IEventRequest ToDomainRequest(Guid userId) => new RoomEventRequest(RoomId, userId, Type, AdditionalData);
}
