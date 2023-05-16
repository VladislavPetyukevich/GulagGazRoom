using Interview.Domain.Repository;

namespace Interview.Domain.Rooms;

public class RoomConfiguration : Entity
{
    public bool EnableCodeEditor { get; set; }

    public string? CodeEditorContent { get; set; }
}
