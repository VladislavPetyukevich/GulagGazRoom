namespace Interview.Domain.Questions;

public interface IQuestionRepository : IRepository<Question>
{
    Task<Question> CreateAsync(QuestionCreateRequest request, CancellationToken cancellationToken = default);
}
