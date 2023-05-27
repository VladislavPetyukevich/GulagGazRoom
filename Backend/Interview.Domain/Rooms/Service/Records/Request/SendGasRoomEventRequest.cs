using Interview.Domain.Events;
using Interview.Domain.Events.Events;

namespace Interview.Domain.Rooms.Service.Records.Request;

public sealed class SendGasRoomEventRequest : IEventRequest
{
    public Guid RoomId { get; }

    public Guid UserId { get; }

    public GasEventType Type { get; }

    public SendGasRoomEventRequest(Guid roomId, Guid userId, GasEventType type)
    {
        RoomId = roomId;
        UserId = userId;
        Type = type;
    }

    public IRoomEvent ToRoomEvent() => new GasRoomEvent(this);

    /// <summary>
    /// Request events.
    /// </summary>
    public enum GasEventType
    {
        /// <summary>
        /// The event is triggered when the admin turns on the gas in the room
        /// </summary>
        GasOn = EventType.GasOn,

        /// <summary>
        /// The event is triggered when the admin turns off the gas in the room
        /// </summary>
        GasOff = EventType.GasOff,
    }

    public class GasRoomEvent : RoomEvent<RoomEventUserPayload>
    {
        public GasRoomEvent(SendGasRoomEventRequest request)
            : base(request.RoomId, (EventType)request.Type, new RoomEventUserPayload(request.UserId))
        {
        }
    }
}
