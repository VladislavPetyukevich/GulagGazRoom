using Interview.Domain.Questions;
using Interview.Infrastructure.Database;

namespace Interview.Infrastructure.Questions;

public class QuestionArchiveRepository : EfArchiveRepository<Question>, IQuestionArchiveRepository
{
    public QuestionArchiveRepository(AppDbContext db)
        : base(db)
    {
    }
}
