using Interview.Domain.Rooms.Service.Records;

namespace Interview.Backend.Rooms;

public sealed class SendGasRoomEventApiRequest
{
    public Guid RoomId { get; set; }

    public SendGasRoomEventRequest.GasEventType Type { get; set; }

    public SendGasRoomEventRequest ToDomainRequest(Guid userId) => new(RoomId, userId, Type);
}
