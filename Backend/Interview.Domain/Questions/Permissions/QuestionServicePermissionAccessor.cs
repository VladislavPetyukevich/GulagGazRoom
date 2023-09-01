using CSharpFunctionalExtensions;
using Interview.Domain.Permissions;
using Interview.Domain.Questions.Records.Response;
using Interview.Domain.ServiceResults.Errors;
using Interview.Domain.ServiceResults.Success;
using X.PagedList;

namespace Interview.Domain.Questions.Permissions;

public class QuestionServicePermissionAccessor : IQuestionService
{
    private readonly IQuestionService _questionService;
    private readonly ISecurityService _securityService;

    public QuestionServicePermissionAccessor(
        IQuestionService questionService, ISecurityService securityService)
    {
        _questionService = questionService;
        _securityService = securityService;
    }

    public Task<IPagedList<QuestionItem>> FindPageAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        _securityService.EnsurePermission(SEPermission.QuestionFindPage);

        return _questionService.FindPageAsync(pageNumber, pageSize, cancellationToken);
    }

    public Task<IPagedList<QuestionItem>> FindPageArchiveAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        _securityService.EnsurePermission(SEPermission.QuestionFindPageArchive);

        return _questionService.FindPageArchiveAsync(pageNumber, pageSize, cancellationToken);
    }

    public Task<QuestionItem> CreateAsync(
        QuestionCreateRequest request, CancellationToken cancellationToken = default)
    {
        _securityService.EnsurePermission(SEPermission.QuestionFindPageArchive);

        return _questionService.CreateAsync(request, cancellationToken);
    }

    public Task<Result<ServiceResult<QuestionItem>, ServiceError>> UpdateAsync(
        Guid id, QuestionEditRequest request, CancellationToken cancellationToken = default)
    {
        _securityService.EnsurePermission(SEPermission.QuestionFindPageArchive);

        return _questionService.UpdateAsync(id, request, cancellationToken);
    }

    public Task<Result<ServiceResult<QuestionItem>, ServiceError>> FindByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        _securityService.EnsurePermission(SEPermission.QuestionFindPageArchive);

        return _questionService.FindByIdAsync(id, cancellationToken);
    }

    public Task<Result<ServiceResult<QuestionItem>, ServiceError>> DeletePermanentlyAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        _securityService.EnsurePermission(SEPermission.QuestionFindPageArchive);

        return _questionService.DeletePermanentlyAsync(id, cancellationToken);
    }

    public Task<Result<ServiceResult<QuestionItem>, ServiceError>> ArchiveAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        _securityService.EnsurePermission(SEPermission.QuestionFindPageArchive);

        return _questionService.ArchiveAsync(id, cancellationToken);
    }

    public Task<Result<ServiceResult<QuestionItem>, ServiceError>> UnarchiveAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        _securityService.EnsurePermission(SEPermission.QuestionFindPageArchive);

        return _questionService.UnarchiveAsync(id, cancellationToken);
    }
}
