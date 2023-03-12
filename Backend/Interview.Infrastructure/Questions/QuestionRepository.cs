using Interview.Domain.Questions;
using Interview.Infrastructure.Database;

namespace Interview.Infrastructure.Questions;

public class QuestionRepository : EfRepository<Question>, IQuestionRepository
{
    public QuestionRepository(AppDbContext db)
        : base(db)
    {
    }

    public async Task<Question> CreateAsync(QuestionCreateRequest request, CancellationToken cancellationToken = default)
    {
        var result = new Question(request.Value);
        await CreateAsync(result, cancellationToken);
        return result;
    }
}
