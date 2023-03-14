using Interview.Domain.Questions;
using Interview.Domain.Repository;
using Interview.Domain.RoomParticipants;

namespace Interview.Domain.Rooms;

public class Room : Entity
{
    public Room(string name)
    {
        Name = name;
    }

    private Room()
        : this(string.Empty)
    {
    }

    public string Name { get; internal set; }

    public List<Question> Questions { get; set; } = new();

    public List<RoomParticipant> Participants { get; set; } = new();
}
