using Interview.Domain.Questions;
using Interview.Infrastructure.Database;

namespace Interview.Infrastructure.Questions;

public class QuestionTagRepository : EfRepository<QuestionTag>, IQuestionTagRepository
{
    public QuestionTagRepository(AppDbContext db)
        : base(db)
    {
    }
}
