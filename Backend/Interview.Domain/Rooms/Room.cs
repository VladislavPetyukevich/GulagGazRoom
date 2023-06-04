using Interview.Domain.Questions;
using Interview.Domain.Repository;
using Interview.Domain.RoomConfigurations;
using Interview.Domain.RoomParticipants;
using Interview.Domain.RoomQuestionReactions;
using Interview.Domain.RoomQuestions;

namespace Interview.Domain.Rooms;

public class Room : Entity
{
    public Room(string name, string twitchChannel)
    {
        Name = name;
        TwitchChannel = twitchChannel;
        Status = SERoomStatus.New;
    }

    private Room()
        : this(string.Empty, string.Empty)
    {
    }

    public string Name { get; internal set; }

    public string TwitchChannel { get; internal set; }

    public SERoomStatus Status { get; internal set; }

    public RoomConfiguration? Configuration { get; set; }

    public List<RoomQuestion> Questions { get; set; } = new();

    public List<RoomParticipant> Participants { get; set; } = new();
}
