using Interview.Domain.Repository;

namespace Interview.Domain.RoomConfigurations;

public class RoomConfiguration : Entity
{
    public bool EnableCodeEditor { get; set; }

    public string? CodeEditorContent { get; set; }
}
