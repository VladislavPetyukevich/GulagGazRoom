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

    protected override IQueryable<RoomQuestion> ApplyIncludes(DbSet<RoomQuestion> set) => Set
        .Include(roomQuestion => roomQuestion.Question)
        .Include(roomQuestion => roomQuestion.Room);
}
