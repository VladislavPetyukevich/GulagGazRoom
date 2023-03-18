using System.Text.Json.Serialization;
using Interview.Domain.Events;
using Interview.Domain.RoomQuestionReactions.Records;

namespace Interview.Backend.RoomReactions;

public class RoomQuestionSendReactionApiRequest
{
    public Guid RoomId { get; set; }

    public Guid QuestionId { get; set; }

    public AvailableEventType Type { get; set; }

    public RoomQuestionSendReactionRequest ToDomainRequest(Guid userId)
    {
        return new RoomQuestionSendReactionRequest(userId, RoomId, QuestionId, (EventType)Type);
    }

    /// <summary>
    /// Available events via API.
    /// </summary>
    public enum AvailableEventType
    {
        /// <summary>
        /// Like event.
        /// </summary>
        Like = EventType.ReactionLike,

        /// <summary>
        /// Dislike event.
        /// </summary>
        Dislike = EventType.ReactionDislike,
    }
}
