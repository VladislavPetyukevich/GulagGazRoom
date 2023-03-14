using Ardalis.SmartEnum;

namespace Interview.Domain.RoomQuestions;

public sealed class RoomQuestionState : SmartEnum<RoomQuestionState>
{
    public static readonly RoomQuestionState Closed = new("Closed", 0);
    public static readonly RoomQuestionState Open = new("Open", 1);
    public static readonly RoomQuestionState Active = new("Active", 2);

    private RoomQuestionState(string name, int value)
        : base(name, value)
    {
    }
}
