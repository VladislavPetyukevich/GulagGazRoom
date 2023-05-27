using Interview.Domain.Events.Events;

namespace Interview.Domain.Rooms.Service.Records.Request;

public interface IEventRequest
{
    Guid RoomId { get; }

    Guid UserId { get; }

    IRoomEvent ToRoomEvent();
}
