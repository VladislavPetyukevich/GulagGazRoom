using Interview.Domain.Questions;
using Interview.Infrastructure.Database;

namespace Interview.Infrastructure.Questions;

public class QuestionRepository : EfRepository<Question>, IQuestionRepository
{
    public QuestionRepository(AppDbContext db) : base(db)
    {
    }
}