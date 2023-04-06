using Interview.Domain.Repository;

namespace Interview.Domain.Questions;

public class Question : ArchiveEntity
{
    public Question(string value)
    {
        Value = value;
    }

    private Question()
        : this(string.Empty)
    {
    }

    public string Value { get; internal set; }
}
