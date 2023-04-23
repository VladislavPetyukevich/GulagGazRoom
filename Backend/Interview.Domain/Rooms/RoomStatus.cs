using Ardalis.SmartEnum;

namespace Interview.Domain.Rooms;

public class RoomStatus : SmartEnum<RoomStatus, char>
{
    public static readonly RoomStatus New = new RoomStatus("Новая", 'N');

    public static readonly RoomStatus Active = new RoomStatus("Активна", 'A');

    public static readonly RoomStatus Review = new RoomStatus("Отзыв", 'R');

    public static readonly RoomStatus Close = new RoomStatus("Закрыта", 'C');

    public RoomStatus(string name, char value)
        : base(name, value)
    {
    }
}
