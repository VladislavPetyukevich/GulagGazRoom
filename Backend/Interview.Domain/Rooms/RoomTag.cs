using Interview.Domain.Tags;

namespace Interview.Domain.Rooms;

public class RoomTag : TagLink
{
    public Guid RoomId { get; internal set; }

    public Room? Room { get; internal set; }
}
