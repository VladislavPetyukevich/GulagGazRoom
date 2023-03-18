using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Interview.Domain.Events.Events;

public interface IRoomEvent
{
    Guid RoomId { get; }

    EventType Type { get; }

    string Stringify()
    {
        return JsonSerializer.Serialize(this, GetType(), new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() },
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        });
    }
}

public interface IRoomEvent<out T> : IRoomEvent
{
    T Value { get; }
}
