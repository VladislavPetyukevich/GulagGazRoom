using System.Diagnostics.CodeAnalysis;

namespace Interview.Domain.Events.Events;

public class RoomEvent : RoomEvent<string>
{
    public RoomEvent(Guid roomId, string type, string? value)
        : base(roomId, type, value)
    {
    }
}

[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Can't have two files with same name")]
public class RoomEvent<T> : IRoomEvent<T>
{
    public Guid RoomId { get; }

    public string Type { get; }

    public T? Value { get; }

    public RoomEvent(Guid roomId, string type, T? value)
    {
        RoomId = roomId;
        Type = type;
        Value = value;
    }
}
