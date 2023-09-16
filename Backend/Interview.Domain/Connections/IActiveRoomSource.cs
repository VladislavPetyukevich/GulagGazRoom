namespace Interview.Domain.Connections;

public interface IActiveRoomSource
{
    ICollection<Guid> ActiveRooms { get; }
}
