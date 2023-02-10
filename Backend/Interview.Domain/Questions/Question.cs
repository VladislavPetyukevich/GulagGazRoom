namespace Interview.Domain.Questions;

public class Question : Entity
{
    public Question(string value)
    {
        Value = value;
    }

    public string Value { get; }
}