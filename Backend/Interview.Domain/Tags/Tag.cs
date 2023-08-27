using Interview.Domain.Questions;
using Interview.Domain.Repository;
using Interview.Domain.Rooms;

namespace Interview.Domain.Tags;

public class Tag : Entity
{
    public string Value { get; internal set; } = string.Empty;

    public List<QuestionTag> QuestionTags { get; internal set; } = new List<QuestionTag>();

    public List<RoomTag> RoomTags { get; internal set; } = new List<RoomTag>();
}
