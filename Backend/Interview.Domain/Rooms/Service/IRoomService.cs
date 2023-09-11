using Interview.Domain.Rooms.Records.Request;
using Interview.Domain.Rooms.Records.Response.RoomStates;
using Interview.Domain.Rooms.Service.Records.Response;
using Interview.Domain.Rooms.Service.Records.Response.Detail;
using Interview.Domain.Rooms.Service.Records.Response.Page;
using Interview.Domain.Rooms.Service.Records.Response.RoomStates;
using X.PagedList;

namespace Interview.Domain.Rooms.Service;

public interface IRoomService : IService
{
    Task<IPagedList<RoomPageDetail>> FindPageAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task<RoomDetail> FindByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Room> CreateAsync(
        RoomCreateRequest request, CancellationToken cancellationToken = default);

    Task<RoomItem> UpdateAsync(
        Guid roomId, RoomUpdateRequest? request, CancellationToken cancellationToken = default);

    Task<Room> AddParticipantAsync(
        Guid roomId, Guid userId, CancellationToken cancellationToken = default);

    Task SendEventRequestAsync(
        IEventRequest request, CancellationToken cancellationToken = default);

    Task CloseAsync(
        Guid roomId,
        CancellationToken cancellationToken = default);

    Task StartReviewAsync(Guid roomId, CancellationToken cancellationToken);

    Task<RoomState> GetStateAsync(
        Guid roomId,
        CancellationToken cancellationToken = default);

    Task<Analytics> GetAnalyticsAsync(
        RoomAnalyticsRequest request,
        CancellationToken cancellationToken = default);

    Task<AnalyticsSummary> GetAnalyticsSummaryAsync(
        RoomAnalyticsRequest request, CancellationToken cancellationToken = default);
}
