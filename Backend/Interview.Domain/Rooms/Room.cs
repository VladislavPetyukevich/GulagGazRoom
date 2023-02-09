using Interview.Domain.Questions;
using Interview.Domain.Users;

namespace Interview.Domain.Rooms;

public class Room : Entity
{
    public Room(string name)
    {
        Name = name;
    }

    private Room() : this("")
    {
    }

    public string Name { get; set; }

    public List<Question> Questions { get; set; } = new();

    public List<User> Users { get; set; } = new();
}