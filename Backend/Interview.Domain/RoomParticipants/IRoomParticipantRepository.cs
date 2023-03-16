using Interview.Domain.Repository;

namespace Interview.Domain.RoomParticipants;

public interface IRoomParticipantRepository : IRepository<RoomParticipant>
{
    public Task<bool> FindByRoomIdAndUserId(Guid roomId, Guid userId);
}
