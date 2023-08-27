using CSharpFunctionalExtensions;
using Interview.Domain.Questions;
using Interview.Domain.Questions.Records.Response;
using Interview.Domain.Repository;
using Interview.Domain.ServiceResults.Errors;
using Interview.Domain.ServiceResults.Success;
using Interview.Domain.Tags.Records.Response;
using NSpecifications;
using X.PagedList;

namespace Interview.Domain.Tags;

public class TagService
{
    private readonly ITagRepository _tagRepository;

    public TagService(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public Task<IPagedList<TagItem>> FindTagsPageAsync(
        string? value, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        value = value?.Trim();
        var mapper =
            new Mapper<Tag, TagItem>(
                question => new TagItem
                {
                    Id = question.Id,
                    Value = question.Value,
                });
        var specification = !string.IsNullOrWhiteSpace(value) ?
            new Spec<Tag>(spec => spec.Value.Contains(value)) :
            Spec<Tag>.Any;
        return _tagRepository.GetPageAsync(specification, mapper, pageNumber, pageSize, cancellationToken);
    }

    public async Task<Result<ServiceResult<TagItem>, ServiceError>> CreateTagAsync(TagEditRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Value))
        {
            return ServiceError.Error("Tag should not be empty");
        }

        request.Value = request.Value.Trim();
        var hasTag = await _tagRepository.HasAsync(new Spec<Tag>(e => e.Value == request.Value), cancellationToken);
        if (hasTag)
        {
            return ServiceError.Error($"Already exists tag '{request.Value}'");
        }

        var tag = new Tag { Value = request.Value, };
        await _tagRepository.CreateAsync(tag, cancellationToken);
        return ServiceResult.Created(new TagItem { Id = tag.Id, Value = request.Value, });
    }

    public async Task<Result<ServiceResult<TagItem>, ServiceError>> UpdateTagAsync(Guid id, TagEditRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Value))
        {
            return ServiceError.Error("Tag should not be empty");
        }

        request.Value = request.Value.Trim();
        var hasTag = await _tagRepository.HasAsync(new Spec<Tag>(e => e.Value == request.Value), cancellationToken);
        if (hasTag)
        {
            return ServiceError.Error($"Already exists tag '{request.Value}'");
        }

        var tag = await _tagRepository.FindByIdAsync(id, cancellationToken);
        if (tag is null)
        {
            return ServiceError.NotFound($"Not found tag by id '{id}'");
        }

        tag.Value = request.Value;
        await _tagRepository.UpdateAsync(tag, cancellationToken);
        return ServiceResult.Ok(new TagItem { Id = tag.Id, Value = request.Value, });
    }
}
