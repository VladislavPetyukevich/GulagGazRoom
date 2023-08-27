using System.Globalization;
using Interview.Domain.Repository;

namespace Interview.Domain.Tags;

public abstract class TagLink : Entity
{
    public Guid TagId { get; internal set; }

    public Tag? Tag { get; internal set; }

    public string HexColor { get; internal set; } = string.Empty;

    public static bool IsValidColor(string color) => int.TryParse(color, NumberStyles.HexNumber, null, out _);
}
