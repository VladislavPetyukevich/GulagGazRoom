namespace Interview.Backend.WebSocket.Events;

public class WebSocketEvent
{
    public string Type { get; set; } = string.Empty;

    public Guid RoomId { get; set; }

    public string Payload { get; set; } = string.Empty;
}
