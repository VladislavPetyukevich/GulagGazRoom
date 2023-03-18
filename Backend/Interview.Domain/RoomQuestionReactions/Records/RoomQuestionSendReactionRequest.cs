using Interview.Domain.Events;

namespace Interview.Domain.RoomQuestionReactions.Records;

public class RoomQuestionSendReactionRequest
{
    public Guid UserId { get; private set; }

    public Guid RoomId { get; private set; }

    public Guid QuestionId { get; private set; }

    public EventType Type { get; private set; }

    public RoomQuestionSendReactionRequest(Guid userId, Guid roomId, Guid questionId, EventType type)
    {
        RoomId = roomId;
        QuestionId = questionId;
        Type = type;
        UserId = userId;
    }
}
