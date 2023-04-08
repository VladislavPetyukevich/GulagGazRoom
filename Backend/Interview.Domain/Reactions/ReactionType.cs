using Ardalis.SmartEnum;
using Interview.Domain.Events;

namespace Interview.Domain.Reactions;

public class ReactionType : SmartEnum<ReactionType>
{
    public static readonly ReactionType Unknown = new(Guid.Empty, "Unknown", 0, EventType.Unknown);

    public static readonly ReactionType Like = new(Guid.Parse("d9a79bbb-5cbb-43d4-80fb-c490e91c333c"), "Like", 1, EventType.ReactionLike);

    public static readonly ReactionType Dislike = new(Guid.Parse("48bfc63a-9498-4438-9211-d2c29d6b3a93"), "Dislike", 2, EventType.ReactionDislike);

    public EventType EventType { get; }

    public Guid Id { get; }

    private ReactionType(Guid id, string name, int value, EventType eventType)
        : base(name, value)
    {
        EventType = eventType;
        Id = id;
    }
}
