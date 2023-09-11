using Interview.Domain.Rooms.Records.Request;
using Interview.Domain.Rooms.Service.Records;

namespace Interview.Backend.Rooms;

public sealed class CodeEditorRoomEventApiRequest
{
    public Guid RoomId { get; set; }

    public CodeEditorRoomEventRequest.CodeEditorRoomEventType Type { get; set; }

    public CodeEditorRoomEventRequest ToDomainRequest(Guid userId) => new(RoomId, userId, Type);
}
