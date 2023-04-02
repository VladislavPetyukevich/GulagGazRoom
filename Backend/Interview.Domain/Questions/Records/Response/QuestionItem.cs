using System.Text.Json.Serialization;

namespace Interview.Domain.Questions.Records.Response;

public class QuestionItem
{
    public Guid Id { get; init; }

    public string Value { get; init; }
}
