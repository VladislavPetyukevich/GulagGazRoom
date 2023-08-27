using Interview.Domain.Questions;
using Interview.Domain.Repository;

namespace Interview.Domain.Tags;

public class Tag : Entity
{
    public string Value { get; internal set; } = string.Empty;

    public List<QuestionTag> QuestionTags { get; internal set; } = new List<QuestionTag>();
}
