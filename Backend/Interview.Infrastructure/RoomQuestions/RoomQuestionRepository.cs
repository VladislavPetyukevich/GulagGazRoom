using System.Linq.Expressions;
using Interview.Domain.RoomQuestions;
using Interview.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Interview.Infrastructure.RoomQuestions;

public class RoomQuestionRepository : EfRepository<RoomQuestion>, IRoomQuestionRepository
{
    public RoomQuestionRepository(AppDbContext db)
        : base(db)
    {
    }

    public Task<RoomQuestion?> FindFirstByQuestionIdAndRoomIdAsync(Guid questionId, Guid roomId, CancellationToken cancellationToken = default)
    {
        return Set
            .FirstOrDefaultAsync(
                roomQuestion => roomQuestion.Room.Id == roomId && roomQuestion.Question.Id == questionId, cancellationToken);
    }

    public Task<RoomQuestion?> FindFirstByRoomAndStateAsync(Guid roomId, RoomQuestionState roomQuestionState, CancellationToken cancellationToken = default)
    {
        return ApplyIncludes(Set)
            .FirstOrDefaultAsync(
                roomQuestion => roomQuestion.Room.Id == roomId && roomQuestion.State == roomQuestionState, cancellationToken);
    }

    public Task<RoomQuestion?> FindFirstQuestionByRoomAndState(Guid roomId, RoomQuestionState roomQuestionState, CancellationToken cancellationToken = default)
    {
        return Set
            .Include(roomQuestion => roomQuestion.Question)
            .Select(roomQuestion => roomQuestion)
            .FirstOrDefaultAsync(
                roomQuestion => roomQuestion.Room.Id == roomId && roomQuestion.State.Name == roomQuestionState.Name, cancellationToken);
    }

    protected override IQueryable<RoomQuestion> ApplyIncludes(DbSet<RoomQuestion> set) => Set
        .Include(roomQuestion => roomQuestion.Question)
        .Include(roomQuestion => roomQuestion.Room);
}
