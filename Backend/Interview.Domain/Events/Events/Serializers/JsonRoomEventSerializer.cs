using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Ardalis.SmartEnum.SystemTextJson;
using Interview.Domain.RoomQuestions;

namespace Interview.Domain.Events.Events.Serializers
{
    public sealed class JsonRoomEventSerializer : IRoomEventSerializer
    {
        private readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            Converters =
            {
                new JsonStringEnumConverter(),
                new SmartEnumNameConverter<RoomQuestionState, int>(),
            },
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        };

        public string SerializeAsString(IRoomEvent @event)
        {
            return JsonSerializer.Serialize(@event, @event.GetType(), _options);
        }
    }
}
