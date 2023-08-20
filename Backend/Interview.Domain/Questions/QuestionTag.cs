using Interview.Domain.Repository;

namespace Interview.Domain.Questions;

public class QuestionTag : Entity
{
    public string Value { get; internal set; } = string.Empty;
}
