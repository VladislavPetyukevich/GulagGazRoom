using Interview.Domain.Rooms.Service.Records;
using Interview.Domain.Rooms.Service.Records.Request;

namespace Interview.Backend.Rooms;

public sealed class CodeEditorRoomEventApiRequest
{
    public Guid RoomId { get; set; }

    public CodeEditorRoomEventRequest.CodeEditorRoomEventType Type { get; set; }

    public CodeEditorRoomEventRequest ToDomainRequest(Guid userId) => new(RoomId, userId, Type);
}
