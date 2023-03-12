using System.Text.Json;
using System.Text.Json.Serialization;

namespace Interview.Domain.Events;

public interface IWebSocketEvent
{
    Guid RoomId { get; }

    EventType Type { get; }

    string Value { get; }

    string Stringify()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() },
        });
    }
}
