namespace Interview.Domain.Tags;

public class TagEditRequest
{
    public string Value { get; set; }

    public string HexColor { get; set; } = string.Empty;
}
