using Ardalis.SmartEnum;

namespace Interview.Domain.RoomUsers;

public sealed class RoomParticipantType : SmartEnum<RoomParticipantType>
{
    public static readonly RoomParticipantType Viewer = new("Viewer", 1);
    public static readonly RoomParticipantType Expert = new("Expert", 2);
    public static readonly RoomParticipantType Examinee = new("Examinee", 3);

    private RoomParticipantType(string name, int value)
        : base(name, value)
    {
    }
}
