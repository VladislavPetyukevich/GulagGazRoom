using Interview.Domain.Questions;
using Interview.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Interview.Infrastructure.Questions;

public class QuestionRepository : EfRepository<Question>, IQuestionRepository
{
    public QuestionRepository(AppDbContext db)
        : base(db)
    {
    }

    protected override IQueryable<Question> ApplyIncludes(DbSet<Question> set) => set;
}
