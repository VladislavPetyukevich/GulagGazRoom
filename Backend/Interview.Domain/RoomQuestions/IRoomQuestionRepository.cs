using Interview.Domain.Repository;

namespace Interview.Domain.RoomQuestions
{
    public interface IRoomQuestionRepository : IRepository<RoomQuestion>
    {
        public Task<RoomQuestion?> FindFirstByRoomAndState(Guid roomId, RoomQuestionState roomQuestionState, CancellationToken cancellationToken = default);

        public Task<RoomQuestion?> FindFirstQuestionByRoomAndState(Guid roomId, RoomQuestionState roomQuestionState, CancellationToken cancellationToken = default);
    }
}
