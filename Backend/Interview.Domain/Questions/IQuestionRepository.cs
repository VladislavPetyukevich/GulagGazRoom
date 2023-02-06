namespace Interview.Domain.Questions;

public interface IQuestionRepository : IRepository<Question>
{

    Task<Question> FindByValueAsync(string value, CancellationToken cancellationToken = default);

}