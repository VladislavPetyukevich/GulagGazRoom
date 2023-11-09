using Interview.Domain.Events.Events;

namespace Interview.Backend.WebSocket.Events;

public class WebSocketEvent : RoomEvent
{
    public WebSocketEvent(Guid roomId, string type, string? value, bool stateful)
        : base(roomId, type, value, stateful)
    {
    }
}
