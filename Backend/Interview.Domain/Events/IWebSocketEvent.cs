namespace Interview.Backend.Controllers.WebSocket
{
    public interface IWebSocketEvent
    {
        Guid RoomId { get; }

        EventType Type { get; }

        string Value { get; }
    }
}
