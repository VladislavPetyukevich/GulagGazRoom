using Interview.Domain.Tags;

namespace Interview.Domain.Questions;

public class QuestionTag : TagLink
{
    public Guid QuestionId { get; internal set; }

    public Question? Question { get; internal set; }
}
