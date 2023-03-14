using System.Text.Json.Serialization;

namespace Interview.Domain.RoomQuestionReactions.Records
{
    public class RoomQuestionReactionCreateRequest
    {
        [JsonPropertyName(name: "reactionId")]
        public Guid ReactionId { get; set; }

        [JsonPropertyName(name: "roomId")]
        public Guid RoomId { get; set; }

        [JsonIgnore]
        public string UserNickname { get; set; }
    }
}
