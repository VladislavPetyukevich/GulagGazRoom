using System.Globalization;
using CSharpFunctionalExtensions;
using Interview.Domain.Questions.Records.Response;
using Interview.Domain.Repository;
using Interview.Domain.ServiceResults.Errors;
using Interview.Domain.ServiceResults.Success;
using Interview.Domain.Tags;
using Interview.Domain.Tags.Records.Response;
using NSpecifications;
using X.PagedList;

namespace Interview.Domain.Questions;

public class QuestionService
{
    private readonly IQuestionRepository _questionRepository;

    private readonly IQuestionNonArchiveRepository _questionNonArchiveRepository;

    private readonly ArchiveService<Question> _archiveService;

    private readonly ITagRepository _tagRepository;

    public QuestionService(
        IQuestionRepository questionRepository,
        IQuestionNonArchiveRepository questionNonArchiveRepository,
        ArchiveService<Question> archiveService,
        ITagRepository tagRepository)
    {
        _questionRepository = questionRepository;
        _questionNonArchiveRepository = questionNonArchiveRepository;
        _archiveService = archiveService;
        _tagRepository = tagRepository;
    }

    public Task<IPagedList<QuestionItem>> FindPageAsync(
        HashSet<Guid> tags, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var mapper =
            new Mapper<Question, QuestionItem>(
                question => new QuestionItem
                {
                    Id = question.Id,
                    Value = question.Value,
                    Tags = question.Tags.Select(e => new LinkedTagItem
                    {
                        Value = e.Tag.Value,
                        HexColor = e.HexColor,
                    }).ToList(),
                });
        var spec = tags.Count == 0
            ? Spec.Any<Question>()
            : new Spec<Question>(e => e.Tags.Any(e => tags.Contains(e.TagId)));
        return _questionNonArchiveRepository.GetPageDetailedAsync(spec, mapper, pageNumber, pageSize, cancellationToken);
    }

    public Task<IPagedList<QuestionItem>> FindPageArchiveAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var mapper = new Mapper<Question, QuestionItem>(question => new QuestionItem
        {
            Id = question.Id,
            Value = question.Value,
            Tags = question.Tags.Select(e => new LinkedTagItem
            {
                Value = e.Tag.Value,
                HexColor = e.HexColor,
            }).ToList(),
        });

        var isArchiveSpecification = new Spec<Question>(question => question.IsArchived);

        return _questionRepository
            .GetPageDetailedAsync(isArchiveSpecification, mapper, pageNumber, pageSize, cancellationToken);
    }

    public async Task<Result<ServiceResult<QuestionItem>, ServiceError>> CreateAsync(
        QuestionCreateRequest request, CancellationToken cancellationToken = default)
    {
        var tags = await EnsureValidTagsAsync(request.Tags, cancellationToken);
        if (tags.IsFailure)
        {
            return tags.Error;
        }

        var result = new Question(request.Value)
        {
            Tags = tags.Value,
        };

        await _questionRepository.CreateAsync(result, cancellationToken);

        return ServiceResult.Created(new QuestionItem
        {
            Id = result.Id,
            Value = result.Value,
            Tags = result.Tags.Select(e => new LinkedTagItem
            {
                Value = e.Tag.Value,
                HexColor = e.HexColor,
            }).ToList(),
        });
    }

    public async Task<Result<ServiceResult<QuestionItem>, ServiceError>> UpdateAsync(
        Guid id, QuestionEditRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _questionNonArchiveRepository.FindByIdAsync(id, cancellationToken);

        if (entity == null)
        {
            return ServiceError.NotFound($"Question not found with id={id}");
        }

        var tags = await EnsureValidTagsAsync(request.Tags, cancellationToken);
        if (tags.IsFailure)
        {
            return tags.Error;
        }

        entity.Value = request.Value;
        entity.Tags.Clear();
        entity.Tags.AddRange(tags.Value);

        await _questionRepository.UpdateAsync(entity, cancellationToken);

        return ServiceResult.Ok(new QuestionItem
        {
            Id = entity.Id,
            Value = entity.Value,
            Tags = entity.Tags.Select(e => new LinkedTagItem
            {
                Value = e.Tag.Value,
                HexColor = e.HexColor,
            }).ToList(),
        });
    }

    public async Task<Result<ServiceResult<QuestionItem>, ServiceError>> FindByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var question = await _questionNonArchiveRepository.FindByIdAsync(id, cancellationToken);

        if (question is null)
        {
            return ServiceError.NotFound($"Not found question with id [{id}]");
        }

        return ServiceResult.Ok(new QuestionItem
        {
            Id = question.Id,
            Value = question.Value,
            Tags = question.Tags.Select(e => new LinkedTagItem
            {
                Value = e.Tag.Value,
                HexColor = e.HexColor,
            }).ToList(),
        });
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

        return ServiceResult.Ok(new QuestionItem
        {
            Id = question.Id,
            Value = question.Value,
            Tags = question.Tags.Select(e => new LinkedTagItem
            {
                Value = e.Tag.Value,
                HexColor = e.HexColor,
            }).ToList(),
        });
    }

    public Task<Result<ServiceResult<QuestionItem>, ServiceError>> ArchiveAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        return _archiveService.ArchiveAsync(id, cancellationToken)
            .Map(q => ServiceResult.Ok(new QuestionItem
            {
                Id = q.Value.Id,
                Value = q.Value.Value,
                Tags = q.Value.Tags.Select(e => new LinkedTagItem
                {
                    Value = e.Tag.Value,
                    HexColor = e.HexColor,
                }).ToList(),
            }));
    }

    public Task<Result<ServiceResult<QuestionItem>, ServiceError>> UnarchiveAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        return _archiveService.UnarchiveAsync(id, cancellationToken)
            .Map(question =>
                ServiceResult.Ok(new QuestionItem
                {
                    Id = question.Value.Id,
                    Value = question.Value.Value,
                    Tags = question.Value.Tags.Select(e => new LinkedTagItem
                    {
                        Value = e.Tag.Value,
                        HexColor = e.HexColor,
                    }).ToList(),
                }));
    }

    private async Task<Result<List<QuestionTag>, ServiceError>> EnsureValidTagsAsync(IReadOnlyCollection<TagLinkRequest> tagsForCheck, CancellationToken cancellationToken)
    {
        var requestTags = tagsForCheck.Select(e => e.TagId).ToHashSet();
        var tags = await _tagRepository.FindByIdsAsync(requestTags, cancellationToken);
        requestTags.ExceptWith(tags.Select(e => e.Id));
        var notFoundTags = string.Join(",", requestTags);
        if (!string.IsNullOrWhiteSpace(notFoundTags))
        {
            return ServiceError.NotFound($"Not found tags: [{notFoundTags}]");
        }

        var notValidHex = tagsForCheck
            .Where(e => !TagLink.IsValidColor(e.HexColor))
            .Take(5);
        var notValidHexStr = string.Join(", ", notValidHex.Select(e => e.TagId.ToString("N") + $" ({e.HexColor})"));
        if (!string.IsNullOrWhiteSpace(notValidHexStr))
        {
            return ServiceError.Error($"The following tags have color set in the wrong format: [{notValidHexStr}]");
        }

        return tags.Join(
                tagsForCheck,
                tag => tag.Id,
                tagRequest => tagRequest.TagId,
                (tag, tagRequest) => new QuestionTag { TagId = tag.Id, HexColor = tagRequest.HexColor })
            .ToList();
    }
}
