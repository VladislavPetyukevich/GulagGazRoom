using Interview.Domain.Permissions;
using Interview.Domain.Rooms.Records.Request;
using Interview.Domain.Rooms.Service;
using Interview.Domain.Rooms.Service.Records.Response;
using Interview.Domain.Rooms.Service.Records.Response.Detail;
using Interview.Domain.Rooms.Service.Records.Response.Page;
using Interview.Domain.Rooms.Service.Records.Response.RoomStates;
using X.PagedList;

namespace Interview.Domain.Rooms.Permissions;

public class RoomServicePermissionAccessor : IRoomService, IServiceDecorator
{
    private readonly IRoomService _roomService;
    private readonly ISecurityService _securityService;

    public RoomServicePermissionAccessor(IRoomService roomService, ISecurityService securityService)
    {
        _roomService = roomService;
        _securityService = securityService;
    }

    public Task<IPagedList<RoomPageDetail>> FindPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        _securityService.EnsurePermission(SEPermission.RoomFindPage);

        return _roomService.FindPageAsync(pageNumber, pageSize, cancellationToken);
    }

    public Task<RoomDetail> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _securityService.EnsurePermission(SEPermission.RoomFindById);

        return _roomService.FindByIdAsync(id, cancellationToken);
    }

    public Task<Room> CreateAsync(RoomCreateRequest request, CancellationToken cancellationToken = default)
    {
        _securityService.EnsurePermission(SEPermission.RoomCreate);

        return _roomService.CreateAsync(request, cancellationToken);
    }

    public Task<RoomItem> UpdateAsync(Guid roomId, RoomUpdateRequest? request, CancellationToken cancellationToken = default)
    {
        _securityService.EnsurePermission(SEPermission.RoomUpdate);

        return _roomService.UpdateAsync(roomId, request, cancellationToken);
    }

    public Task<Room> AddParticipantAsync(Guid roomId, Guid userId, CancellationToken cancellationToken = default)
    {
        _securityService.EnsurePermission(SEPermission.RoomAddParticipant);

        return _roomService.AddParticipantAsync(roomId, userId, cancellationToken);
    }

    public Task SendEventRequestAsync(IEventRequest request, CancellationToken cancellationToken = default)
    {
        _securityService.EnsurePermission(SEPermission.RoomSendEventRequest);

        return _roomService.SendEventRequestAsync(request, cancellationToken);
    }

    public Task CloseAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        _securityService.EnsurePermission(SEPermission.RoomClose);

        return _roomService.CloseAsync(roomId, cancellationToken);
    }

    public Task StartReviewAsync(Guid roomId, CancellationToken cancellationToken)
    {
        _securityService.EnsurePermission(SEPermission.RoomStartReview);

        return _roomService.StartReviewAsync(roomId, cancellationToken);
    }

    public Task<RoomState> GetStateAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        _securityService.EnsurePermission(SEPermission.RoomGetState);

        return _roomService.GetStateAsync(roomId, cancellationToken);
    }

    public Task<Analytics> GetAnalyticsAsync(RoomAnalyticsRequest request, CancellationToken cancellationToken = default)
    {
        _securityService.EnsurePermission(SEPermission.RoomGetAnalytics);

        return _roomService.GetAnalyticsAsync(request, cancellationToken);
    }

    public Task<AnalyticsSummary> GetAnalyticsSummaryAsync(RoomAnalyticsRequest request, CancellationToken cancellationToken = default)
    {
        _securityService.EnsurePermission(SEPermission.RoomGetAnalyticsSummary);

        return _roomService.GetAnalyticsSummaryAsync(request, cancellationToken);
    }
}
