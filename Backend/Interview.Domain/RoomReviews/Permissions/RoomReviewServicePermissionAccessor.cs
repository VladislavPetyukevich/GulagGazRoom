using Interview.Domain.Permissions;
using Interview.Domain.RoomReviews.Records;
using Interview.Domain.RoomReviews.Services;
using Interview.Domain.Rooms.Service.Records.Response.Page;
using X.PagedList;

namespace Interview.Domain.RoomReviews.Permissions;

public class RoomReviewServicePermissionAccessor : IRoomReviewService
{
    private readonly IRoomReviewService _roomReviewService;
    private readonly ISecurityService _securityService;

    public RoomReviewServicePermissionAccessor(IRoomReviewService roomReviewService, ISecurityService securityService)
    {
        _roomReviewService = roomReviewService;
        _securityService = securityService;
    }

    public Task<IPagedList<RoomReviewPageDetail>> FindPageAsync(RoomReviewPageRequest request, CancellationToken cancellationToken = default)
    {
        _securityService.EnsurePermission(SEPermission.RoomReviewFindPage);

        return _roomReviewService.FindPageAsync(request, cancellationToken);
    }

    public Task<RoomReviewDetail> CreateAsync(RoomReviewCreateRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        _securityService.EnsurePermission(SEPermission.RoomReviewCreate);

        return _roomReviewService.CreateAsync(request, userId, cancellationToken);
    }

    public Task<RoomReviewDetail> UpdateAsync(Guid id, RoomReviewUpdateRequest request, CancellationToken cancellationToken = default)
    {
        _securityService.EnsurePermission(SEPermission.RoomReviewUpdate);

        return _roomReviewService.UpdateAsync(id, request, cancellationToken);
    }
}
