using Interview.Domain.Rooms;
using Interview.Domain.Users;
using Entity = Interview.Domain.Repository.Entity;

namespace Interview.Domain.RoomUsers;

public class RoomParticipant : Entity
{
    private RoomParticipant() {}

    public RoomParticipant(User user, Room room, RoomParticipantType type)
    {
        User = user;
        Room = room;
        Type = type;
    }

    public User User { get; set; } = null!;

    public Room Room { get; set; } = null!;

    public RoomParticipantType Type { get; set; } = null!;
}
