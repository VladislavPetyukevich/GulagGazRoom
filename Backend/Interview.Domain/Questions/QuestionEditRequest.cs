using Interview.Domain.Tags;

namespace Interview.Domain.Questions;

public sealed class QuestionEditRequest
{
    public string Value { get; set; } = string.Empty;

    public List<TagLinkRequest> Tags { get; set; } = new List<TagLinkRequest>();
}
