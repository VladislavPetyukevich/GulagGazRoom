using Interview.Domain.Repository;

namespace Interview.Domain.Questions;

public interface IQuestionRepository : IRepository<Question>
{
    public Task DeletePermanentlyAsync(Question entity, CancellationToken cancellationToken = default);
}
