using CSharpFunctionalExtensions;
using Interview.Domain.Questions.Records.Response;
using Interview.Domain.ServiceResults.Errors;
using Interview.Domain.ServiceResults.Success;
using X.PagedList;

namespace Interview.Domain.Questions.Permissions
{
    public interface IQuestionService
    {
        Task<IPagedList<QuestionItem>> FindPageAsync(
            int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<IPagedList<QuestionItem>> FindPageArchiveAsync(
            int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<Result<ServiceResult<QuestionItem>, ServiceError>> CreateAsync(
            QuestionCreateRequest request, CancellationToken cancellationToken = default);

        Task<Result<ServiceResult<QuestionItem>, ServiceError>> UpdateAsync(
            Guid id, QuestionEditRequest request, CancellationToken cancellationToken = default);

        Task<Result<ServiceResult<QuestionItem>, ServiceError>> FindByIdAsync(
            Guid id, CancellationToken cancellationToken = default);

        Task<Result<ServiceResult<QuestionItem>, ServiceError>> DeletePermanentlyAsync(
            Guid id, CancellationToken cancellationToken = default);

        Task<Result<ServiceResult<QuestionItem>, ServiceError>> ArchiveAsync(
            Guid id, CancellationToken cancellationToken = default);

        Task<Result<ServiceResult<QuestionItem>, ServiceError>> UnarchiveAsync(
            Guid id, CancellationToken cancellationToken = default);
    }
}
