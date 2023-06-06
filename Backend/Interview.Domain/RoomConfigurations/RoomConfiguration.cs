using Interview.Domain.Repository;
using Interview.Domain.Rooms;

namespace Interview.Domain.RoomConfigurations;

public class RoomConfiguration : Entity
{
    public bool EnableCodeEditor { get; set; }

    public string? CodeEditorContent { get; set; }

    public Room? Room { get; set; }
}
