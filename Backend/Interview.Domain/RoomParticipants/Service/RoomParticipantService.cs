namespace Interview.Domain.RoomParticipants.Service;

public class RoomParticipantService
{
    private readonly IRoomParticipantRepository _roomParticipantRepository;

    public RoomParticipantService(IRoomParticipantRepository roomParticipantRepository)
    {
        _roomParticipantRepository = roomParticipantRepository;
    }

    public Task<bool> IsAlreadyRegistry(Guid roomId, Guid userId)
    {
        return _roomParticipantRepository.FindByRoomIdAndUserId(roomId, userId);
    }
}
