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

    private readonly IQuestionNonArchiveRepository _questionNonArchiveRepository;

    private readonly ArchiveService<Question> _archiveService;

    private readonly IQuestionTagRepository _questionTagRepository;

    public QuestionService(
        IQuestionRepository questionRepository,
        IQuestionNonArchiveRepository questionNonArchiveRepository,
        ArchiveService<Question> archiveService,
        IQuestionTagRepository questionTagRepository)
    {
        _questionRepository = questionRepository;
        _questionNonArchiveRepository = questionNonArchiveRepository;
        _archiveService = archiveService;
        _questionTagRepository = questionTagRepository;
    }

    public Task<IPagedList<QuestionTagItem>> FindTagsPageAsync(
        string? value, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        value = value?.Trim();
        var mapper =
            new Mapper<QuestionTag, QuestionTagItem>(
                question => new QuestionTagItem { Id = question.Id, Value = question.Value, });
        var specification = !string.IsNullOrWhiteSpace(value) ?
            new Spec<QuestionTag>(spec => spec.Value.Contains(value)) :
            Spec<QuestionTag>.Any;
        return _questionTagRepository.GetPageAsync(specification, mapper, pageNumber, pageSize, cancellationToken);
    }

    public async Task<Result<ServiceResult<QuestionTagItem>, ServiceError>> CreateTagAsync(QuestionTagEditRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Value))
        {
            return ServiceError.Error("Tag should not be empty");
        }

        request.Value = request.Value.Trim();
        var hasTag = await _questionTagRepository.HasAsync(new Spec<QuestionTag>(e => e.Value == request.Value), cancellationToken);
        if (hasTag)
        {
            return ServiceError.Error($"Already exists tag '{request.Value}'");
        }

        var tag = new QuestionTag { Value = request.Value, };
        await _questionTagRepository.CreateAsync(tag, cancellationToken);
        return ServiceResult.Created(new QuestionTagItem { Id = tag.Id, Value = request.Value, });
    }

    public async Task<Result<ServiceResult<QuestionTagItem>, ServiceError>> UpdateTagAsync(Guid id, QuestionTagEditRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Value))
        {
            return ServiceError.Error("Tag should not be empty");
        }

        request.Value = request.Value.Trim();
        var hasTag = await _questionTagRepository.HasAsync(new Spec<QuestionTag>(e => e.Value == request.Value), cancellationToken);
        if (hasTag)
        {
            return ServiceError.Error($"Already exists tag '{request.Value}'");
        }

        var tag = await _questionTagRepository.FindByIdAsync(id, cancellationToken);
        if (tag is null)
        {
            return ServiceError.NotFound($"Not found tag by id '{id}'");
        }

        tag.Value = request.Value;
        await _questionTagRepository.UpdateAsync(tag, cancellationToken);
        return ServiceResult.Ok(new QuestionTagItem { Id = tag.Id, Value = request.Value, });
    }

    public Task<IPagedList<QuestionItem>> FindPageAsync(
        HashSet<Guid> tags, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var mapper =
            new Mapper<Question, QuestionItem>(
                question => new QuestionItem { Id = question.Id, Value = question.Value, Tags = question.Tags.Select(e => e.Value).ToList(), });
        var spec = tags.Count == 0
            ? Spec.Any<Question>()
            : new Spec<Question>(e => e.Tags.Any(e => tags.Contains(e.Id)));
        return _questionNonArchiveRepository.GetPageDetailedAsync(spec, mapper, pageNumber, pageSize, cancellationToken);
    }

    public Task<IPagedList<QuestionItem>> FindPageArchiveAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var mapper = new Mapper<Question, QuestionItem>(question => new QuestionItem
        {
            Id = question.Id,
            Value = question.Value,
            Tags = question.Tags.Select(e => e.Value).ToList(),
        });

        var isArchiveSpecification = new Spec<Question>(question => question.IsArchived);

        return _questionRepository
            .GetPageDetailedAsync(isArchiveSpecification, mapper, pageNumber, pageSize, cancellationToken);
    }

    public async Task<Result<ServiceResult<QuestionItem>, ServiceError>> CreateAsync(
        QuestionCreateRequest request, CancellationToken cancellationToken = default)
    {
        var tags = await _questionTagRepository.FindByIdsAsync(request.Tags, cancellationToken);
        var notFoundTags = string.Join(",", request.Tags.Except(tags.Select(e => e.Id)));
        if (!string.IsNullOrWhiteSpace(notFoundTags))
        {
            return ServiceError.NotFound($"Not found tags: [{notFoundTags}]");
        }

        var result = new Question(request.Value)
        {
            Tags = tags,
        };

        await _questionRepository.CreateAsync(result, cancellationToken);

        return ServiceResult.Created(new QuestionItem { Id = result.Id, Value = result.Value, Tags = result.Tags.Select(e => e.Value).ToList(), });
    }

    public async Task<Result<ServiceResult<QuestionItem>, ServiceError>> UpdateAsync(
        Guid id, QuestionEditRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _questionNonArchiveRepository.FindByIdAsync(id, cancellationToken);

        if (entity == null)
        {
            return ServiceError.NotFound($"Question not found with id={id}");
        }

        var tags = await _questionTagRepository.FindByIdsAsync(request.Tags, cancellationToken);
        var notFoundTags = string.Join(",", request.Tags.Except(tags.Select(e => e.Id)));
        if (!string.IsNullOrWhiteSpace(notFoundTags))
        {
            return ServiceError.NotFound($"Not found tags: [{notFoundTags}]");
        }

        entity.Value = request.Value;
        entity.Tags.Clear();
        entity.Tags.AddRange(tags);

        await _questionRepository.UpdateAsync(entity, cancellationToken);

        return ServiceResult.Ok(new QuestionItem { Id = entity.Id, Value = entity.Value, Tags = entity.Tags.Select(e => e.Value).ToList(), });
    }

    public async Task<Result<ServiceResult<QuestionItem>, ServiceError>> FindByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var question = await _questionNonArchiveRepository.FindByIdAsync(id, cancellationToken);

        if (question is null)
        {
            return ServiceError.NotFound($"Not found question with id [{id}]");
        }

        return ServiceResult.Ok(new QuestionItem { Id = question.Id, Value = question.Value, Tags = question.Tags.Select(e => e.Value).ToList(), });
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

        return ServiceResult.Ok(new QuestionItem { Id = question.Id, Value = question.Value, Tags = question.Tags.Select(e => e.Value).ToList(), });
    }

    public Task<Result<ServiceResult<QuestionItem>, ServiceError>> ArchiveAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        return _archiveService.ArchiveAsync(id, cancellationToken)
            .Map(q => ServiceResult.Ok(new QuestionItem { Id = q.Value.Id, Value = q.Value.Value, Tags = q.Value.Tags.Select(e => e.Value).ToList(), }));
    }

    public Task<Result<ServiceResult<QuestionItem>, ServiceError>> UnarchiveAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        return _archiveService.UnarchiveAsync(id, cancellationToken)
            .Map(question =>
                ServiceResult.Ok(new QuestionItem { Id = question.Value.Id, Value = question.Value.Value, Tags = question.Value.Tags.Select(e => e.Value).ToList(), }));
    }
}
