using Interview.Domain.Events;
using Interview.Domain.Events.Events;

namespace Interview.Domain.Rooms.Records.Request;

public sealed class CodeEditorRoomEventRequest : IEventRequest
{
    public Guid RoomId { get; }

    public Guid UserId { get; }

    public CodeEditorRoomEventType Type { get; }

    public CodeEditorRoomEventRequest(Guid roomId, Guid userId, CodeEditorRoomEventType type)
    {
        RoomId = roomId;
        UserId = userId;
        Type = type;
    }

    public IRoomEvent ToRoomEvent() => new CodeEditorRoomEvent(this);

    /// <summary>
    /// Request events.
    /// </summary>
    public enum CodeEditorRoomEventType
    {
        /// <summary>
        /// The event is triggered when enable code editor for room
        /// </summary>
        EnableCodeEditor = EventType.EnableCodeEditor,

        /// <summary>
        /// The event is triggered when disable code editor for room
        /// </summary>
        DisableCodeEditor = EventType.DisableCodeEditor,
    }

    public class CodeEditorRoomEvent : RoomEvent<RoomEventUserPayload>
    {
        public CodeEditorRoomEvent(CodeEditorRoomEventRequest request)
            : base(request.RoomId, (EventType)request.Type, new RoomEventUserPayload(request.UserId))
        {
        }
    }
}
