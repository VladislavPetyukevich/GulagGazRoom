using Interview.Domain.Events.Events;

namespace Interview.Backend.WebSocket.Events;

public class WebSocketEvent : RoomEvent
{
    public string Type { get; set; } = string.Empty;

    public string Payload { get; set; } = string.Empty;

    public WebSocketEvent(Guid roomId, string type, string? value, bool stateful)
        : base(roomId, type, value, stateful)
    {
    }
}
