using Ardalis.SmartEnum;

namespace Interview.Domain.Reactions;

public class ReactionType : SmartEnum<ReactionType>
{
    public static readonly ReactionType Unknown = new ("Неизвестно", 0);

    public static readonly ReactionType Like = new ("Нравится", 1);

    public static readonly ReactionType Dislike = new ("Не нравится", 2);

    public static readonly ReactionType Average = new ("Средне", 3);

    private ReactionType(string name, int value)
        : base(name, value)
    {
    }
}
