using Interview.Domain.Repository;

namespace Interview.Domain.RoomQuestions;

public interface IRoomQuestionRepository : IRepository<RoomQuestion>
{
    public Task<RoomQuestion?> FindFirstByQuestionIdAndRoomIdAsync(Guid questionId, Guid roomId, CancellationToken cancellationToken);

    public Task<RoomQuestion?> FindFirstByRoomAndStateAsync(Guid roomId, RoomQuestionState roomQuestionState, CancellationToken cancellationToken = default);
}
