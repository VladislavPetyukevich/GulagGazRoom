using Interview.Domain.Questions;
using Interview.Domain.Reactions;
using Interview.Domain.Users;

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

    public List<User> Users { get; set; } = new();

    public List<Reaction> Reactions { get; set; } = new();
}
