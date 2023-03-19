using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Ardalis.SmartEnum.SystemTextJson;
using Interview.Domain.RoomQuestions;

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
            Converters =
            {
                new JsonStringEnumConverter(),
                new SmartEnumNameConverter<RoomQuestionState, int>(),
            },
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        });
    }
}

public interface IRoomEvent<out T> : IRoomEvent
{
    T Value { get; }
}
