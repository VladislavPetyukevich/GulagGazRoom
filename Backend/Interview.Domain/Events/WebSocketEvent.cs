namespace Interview.Domain.Events
{
    public sealed class WebSocketEvent : IWebSocketEvent
    {
        public Guid RoomId { get; }

        public EventType Type { get; }

        public string Value { get; }

        public WebSocketEvent(Guid roomId, EventType type, string value)
        {
            RoomId = roomId;
            Type = type;
            Value = value;
        }
    }
}
