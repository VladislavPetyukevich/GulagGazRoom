using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Interview.Domain.Events.Events;

public class RoomEvent : RoomEvent<string>
{
    public RoomEvent(Guid roomId, string type, string? value, bool stateful)
        : base(roomId, type, value, stateful)
    {
    }
}

[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Can't have two files with same name")]
public class RoomEvent<T> : IRoomEvent<T>
{
    public Guid RoomId { get; }

    public string Type { get; }

    public bool Stateful { get; }

    public T? Value { get; }

    public RoomEvent(Guid roomId, string type, T? value, bool stateful)
    {
        RoomId = roomId;
        Type = type;
        Value = value;
        Stateful = stateful;
    }

    public string? BuildStringPayload()
    {
        if (Value is null)
        {
            return null;
        }

        if (typeof(T) == typeof(string))
        {
            return (string)(object)Value;
        }

        return JsonSerializer.Serialize(Value);
    }
}
