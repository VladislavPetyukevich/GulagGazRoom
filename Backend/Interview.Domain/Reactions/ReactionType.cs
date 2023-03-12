using Ardalis.SmartEnum;

namespace Interview.Domain.Reactions;

public class ReactionType : SmartEnum<ReactionType>
{
    public static readonly ReactionType Unknown = new("Unknown", 0);

    public static readonly ReactionType Like = new("Like", 1);

    public static readonly ReactionType Dislike = new("Dislike", 2);

    public static readonly ReactionType Average = new("Average", 3);

    private ReactionType(string name, int value)
        : base(name, value)
    {
    }
}
