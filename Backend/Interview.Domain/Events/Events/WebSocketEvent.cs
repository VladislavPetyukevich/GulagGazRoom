using System.Diagnostics.CodeAnalysis;

namespace Interview.Domain.Events.Events;

public sealed class WebSocketEvent : WebSocketEvent<string>
{
    public WebSocketEvent(Guid roomId, EventType type, string value)
        : base(roomId, type, value)
    {
    }
}

[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Can't have two files with same name")]
public class WebSocketEvent<T> : IWebSocketEvent<T>
{
    public Guid RoomId { get; }

    public EventType Type { get; }

    public T Value { get; }

    public WebSocketEvent(Guid roomId, EventType type, T value)
    {
        RoomId = roomId;
        Type = type;
        Value = value;
    }
}
