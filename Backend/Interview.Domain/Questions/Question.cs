namespace Interview.Domain.Questions;

public class Question : Entity
{
    public string Value { get; private set; }

    public Question(string value)
    {
        Value = value;
    }
    
}