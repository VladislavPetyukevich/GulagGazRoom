using CSharpFunctionalExtensions;
using Interview.Domain.Questions.Records.Response;
using Interview.Domain.Repository;
using Interview.Domain.ServiceResults;
using Interview.Domain.ServiceResults.Errors;
using Interview.Domain.ServiceResults.Success;
using X.PagedList;

namespace Interview.Domain.Questions;

public class QuestionService
{
    private readonly IQuestionRepository _questionRepository;

    public QuestionService(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public Task<IPagedList<QuestionItem>> FindPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var mapper = new Mapper<Question, QuestionItem>(question => new QuestionItem { Id = question.Id, Value = question.Value });
        return _questionRepository.GetPageDetailedAsync(mapper, pageNumber, pageSize, cancellationToken);
    }

    public async Task<Result<ServiceResult<QuestionItem>, ServiceError>> CreateAsync(QuestionCreateRequest request, CancellationToken cancellationToken = default)
    {
        var result = new Question(request.Value);

        await _questionRepository.CreateAsync(result, cancellationToken);

        return ServiceResult.Created(new QuestionItem
        {
            Id = result.Id,
            Value = result.Value,
        });
    }

    public async Task<Result<ServiceResult<QuestionItem>, ServiceError>> UpdateAsync(Guid id, QuestionEditRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _questionRepository.FindByIdAsync(id, cancellationToken);

        if (entity == null)
        {
            return ServiceError.NotFound($"Question not found with id={id}");
        }

        entity.Value = request.Value;

        await _questionRepository.UpdateAsync(entity, cancellationToken);

        return ServiceResult.Ok(new QuestionItem
        {
            Id = entity.Id,
            Value = entity.Value,
        });
    }

    public async Task<Result<ServiceResult<QuestionItem>, ServiceError>> FindById(Guid id, CancellationToken cancellationToken = default)
    {
        var question = await _questionRepository.FindByIdAsync(id, cancellationToken);
        if (question is null)
        {
            return ServiceError.NotFound($"Not found question with id [{id}]");
        }

        return ServiceResult.Ok(new QuestionItem { Id = question.Id, Value = question.Value });
    }
}
