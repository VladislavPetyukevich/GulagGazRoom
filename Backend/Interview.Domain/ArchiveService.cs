using CSharpFunctionalExtensions;
using Interview.Domain.Repository;
using Interview.Domain.ServiceResults.Errors;
using Interview.Domain.ServiceResults.Success;

namespace Interview.Domain;

public class ArchiveService<T>
    where T : ArchiveEntity
{
    private readonly IRepository<T> _repository;

    public ArchiveService(IRepository<T> repository)
    {
        _repository = repository;
    }

    public async Task<Result<ServiceResult<T>, ServiceError>> ArchiveAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var question = await _repository.FindByIdAsync(id, cancellationToken);

        if (question == null)
        {
            return ServiceError.NotFound($"{typeof(T).Name} not found by id {id}");
        }

        question.IsArchived = true;

        await _repository.UpdateAsync(question, cancellationToken);

        return ServiceResult.Ok(question);
    }

    public async Task<Result<ServiceResult<T>, ServiceError>> UnarchiveAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var question = await _repository.FindByIdAsync(id, cancellationToken);

        if (question == null)
        {
            return ServiceError.NotFound($"{typeof(T).Name} not found by id {id}");
        }

        if (!question.IsArchived)
        {
            return ServiceError.Error($"The {typeof(T).Name} is not archived");
        }

        question.IsArchived = false;

        await _repository.UpdateAsync(question, cancellationToken);

        return ServiceResult.Ok(question);
    }
}
