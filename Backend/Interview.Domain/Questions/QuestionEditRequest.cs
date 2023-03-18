namespace Interview.Domain.Questions;

public sealed class QuestionEditRequest
{
    public Guid Id { get; set; }

    public string Value { get; set; } = string.Empty;
}
