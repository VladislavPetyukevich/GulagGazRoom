using Ardalis.SmartEnum;
using Interview.Domain.Events;

namespace Interview.Domain.Reactions;

public class ReactionType : SmartEnum<ReactionType>
{
    public static readonly ReactionType Unknown = new("Unknown", 0, EventType.Unknown);

    public static readonly ReactionType Like = new("Like", 1, EventType.ReactionLike);

    public static readonly ReactionType Dislike = new("Dislike", 2, EventType.ReactionDislike);

    public EventType EventType { get; }

    private ReactionType(string name, int value, EventType eventType)
        : base(name, value)
    {
        EventType = eventType;
    }
}
