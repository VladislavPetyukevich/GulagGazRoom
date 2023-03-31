using CSharpFunctionalExtensions;
using Interview.Domain.Questions.Records.Response;
using Interview.Domain.Repository;
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

    public async Task<Result<QuestionItem?>> CreateAsync(QuestionCreateRequest request, CancellationToken cancellationToken = default)
    {
        var result = new Question(request.Value);

        await _questionRepository.CreateAsync(result, cancellationToken);

        return new QuestionItem
        {
            Id = result.Id,
            Value = result.Value,
        };
    }

    public async Task<Result<QuestionItem?>> UpdateAsync(Guid id, QuestionEditRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _questionRepository.FindByIdAsync(id, cancellationToken);

        if (entity == null)
        {
            return Result.Failure<QuestionItem?>($"Question not found with id={id}");
        }

        entity.Value = request.Value;

        await _questionRepository.UpdateAsync(entity, cancellationToken);

        return new QuestionItem
        {
            Id = entity.Id,
            Value = entity.Value,
        };
    }

    public async Task<Result<QuestionItem>> FindById(Guid id, CancellationToken cancellationToken = default)
    {
        var question = await _questionRepository.FindByIdAsync(id, cancellationToken);

        return question == null
            ? Result.Failure<QuestionItem>($"Not found question with id [{id}]")
            : new QuestionItem { Id = question.Id, Value = question.Value };
    }
}
