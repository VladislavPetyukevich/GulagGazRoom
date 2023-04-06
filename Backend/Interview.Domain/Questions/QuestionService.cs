using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using Interview.Domain.Questions.Records.Response;
using Interview.Domain.Repository;
using Interview.Domain.ServiceResults.Errors;
using Interview.Domain.ServiceResults.Success;
using NSpecifications;
using X.PagedList;

namespace Interview.Domain.Questions;

public class QuestionService
{
    private readonly IQuestionRepository _questionRepository;

    private readonly IQuestionArchiveRepository _questionArchiveRepository;

    public QuestionService(IQuestionRepository questionRepository, IQuestionArchiveRepository questionArchiveRepository)
    {
        _questionRepository = questionRepository;
        _questionArchiveRepository = questionArchiveRepository;
    }

    public Task<IPagedList<QuestionItem>> FindPageAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var mapper =
            new Mapper<Question, QuestionItem>(
                question => new QuestionItem { Id = question.Id, Value = question.Value });
        return _questionArchiveRepository.GetPageDetailedAsync(mapper, pageNumber, pageSize, cancellationToken);
    }

    public Task<IPagedList<QuestionItem>> FindPageArchiveAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var mapper = new Mapper<Question, QuestionItem>(question => new QuestionItem
        {
            Id = question.Id,
            Value = question.Value,
        });

        var isArchiveSpecification = new Spec<Question>(question => question.IsArchived);

        return _questionRepository.GetPageDetailedAsync(isArchiveSpecification, mapper, pageNumber, pageSize, cancellationToken);
    }

    public async Task<Result<ServiceResult<QuestionItem>, ServiceError>> CreateAsync(
        QuestionCreateRequest request, CancellationToken cancellationToken = default)
    {
        var result = new Question(request.Value);

        await _questionRepository.CreateAsync(result, cancellationToken);

        return ServiceResult.Created(new QuestionItem { Id = result.Id, Value = result.Value, });
    }

    public async Task<Result<ServiceResult<QuestionItem>, ServiceError>> UpdateAsync(
        Guid id, QuestionEditRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _questionArchiveRepository.FindByIdAsync(id, cancellationToken);

        if (entity == null)
        {
            return ServiceError.NotFound($"Question not found with id={id}");
        }

        entity.Value = request.Value;

        await _questionRepository.UpdateAsync(entity, cancellationToken);

        return ServiceResult.Ok(new QuestionItem { Id = entity.Id, Value = entity.Value, });
    }

    public async Task<Result<ServiceResult<QuestionItem>, ServiceError>> FindByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var question = await _questionArchiveRepository.FindByIdAsync(id, cancellationToken);

        if (question is null)
        {
            return ServiceError.NotFound($"Not found question with id [{id}]");
        }

        return ServiceResult.Ok(new QuestionItem { Id = question.Id, Value = question.Value });
    }

    /// <summary>
    /// Permanent deletion of a question with verification of its existence.
    /// </summary>
    /// <param name="id">Question's guid.</param>
    /// <param name="cancellationToken">Cancellation Token.</param>
    /// <returns>ServiceResult with QuestionItem - success, ServiceError - error.</returns>
    public async Task<Result<ServiceResult<QuestionItem>, ServiceError>> DeletePermanentlyAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var question = await _questionRepository.FindByIdAsync(id, cancellationToken);

        if (question == null)
        {
            return ServiceError.NotFound($"Question not found by id {id}");
        }

        await _questionRepository.DeletePermanentlyAsync(question, cancellationToken);

        return ServiceResult.Ok(new QuestionItem { Id = question.Id, Value = question.Value, });
    }

    public async Task<Result<ServiceResult<QuestionItem>, ServiceError>> ArchiveAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var question = await _questionRepository.FindByIdAsync(id, cancellationToken);

        if (question == null)
        {
            return ServiceError.NotFound($"Question not found by id {id}");
        }

        question.IsArchived = true;

        await _questionRepository.UpdateAsync(question, cancellationToken);

        return ServiceResult.Ok(new QuestionItem { Id = question.Id, Value = question.Value, });
    }

    public async Task<Result<ServiceResult<QuestionItem>, ServiceError>> UnarchiveAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var question = await _questionRepository.FindByIdAsync(id, cancellationToken);

        if (question == null)
        {
            return ServiceError.NotFound($"Question not found by id {id}");
        }

        if (!question.IsArchived)
        {
            return ServiceError.Error($"The question is not archived");
        }

        question.IsArchived = false;

        await _questionRepository.UpdateAsync(question, cancellationToken);

        return ServiceResult.Ok(new QuestionItem { Id = question.Id, Value = question.Value, });
    }
}
