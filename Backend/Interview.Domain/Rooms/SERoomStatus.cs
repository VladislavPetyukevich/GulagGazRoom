using Ardalis.SmartEnum;

namespace Interview.Domain.Rooms;

public class SERoomStatus : SmartEnum<SERoomStatus, char>
{
    public static readonly SERoomStatus New = new SERoomStatus("Новая", 'N', EVRoomStatus.New);

    public static readonly SERoomStatus Active = new SERoomStatus("Активна", 'A', EVRoomStatus.Active);

    public static readonly SERoomStatus Review = new SERoomStatus("Отзыв", 'R', EVRoomStatus.Review);

    public static readonly SERoomStatus Close = new SERoomStatus("Закрыта", 'C', EVRoomStatus.Close);

    public SERoomStatus(string name, char value, EVRoomStatus enumValue)
        : base(name, value)
    {
        EnumValue = enumValue;
    }

    public EVRoomStatus EnumValue { get; }
}
