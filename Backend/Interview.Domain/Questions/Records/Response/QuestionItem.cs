using System.Text.Json.Serialization;
using Interview.Domain.Tags;
using Interview.Domain.Tags.Records.Response;

namespace Interview.Domain.Questions.Records.Response;

public class QuestionItem
{
    public Guid Id { get; init; }

    public string Value { get; init; } = string.Empty;

    public required List<LinkedTagItem> Tags { get; init; }
}
