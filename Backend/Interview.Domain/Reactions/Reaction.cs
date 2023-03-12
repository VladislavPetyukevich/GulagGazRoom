using Interview.Domain.Rooms;
using Interview.Domain.Users;

namespace Interview.Domain.Reactions;

public class Reaction : Entity
{
    public ReactionType Type { get; set; } = ReactionType.Unknown;

    public Guid FromUserId { get; set; }

    public Guid ToUserId { get; set; }

    public Guid RoomId { get; set; }

    public User? FromUser { get; set; }

    public User? ToUser { get; set; }

    public Room? Room { get; set; }
}
