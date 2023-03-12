namespace Interview.Domain.Questions;

public class Question : Entity
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
