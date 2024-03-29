using Interview.Domain.Repository;
using Interview.Domain.RoomParticipants;
using Interview.Domain.Rooms.Records.Request;
using Interview.Domain.Rooms.Service.Records.Response.Detail;
using Interview.Domain.Rooms.Service.Records.Response.Page;
using X.PagedList;

namespace Interview.Domain.Rooms;

public interface IRoomRepository : IRepository<Room>
{
    Task<AnalyticsSummary?> GetAnalyticsSummaryAsync(RoomAnalyticsRequest request, CancellationToken cancellationToken = default);

    Task<Analytics?> GetAnalyticsAsync(RoomAnalyticsRequest request, CancellationToken cancellationToken = default);

    Task<bool> HasUserAsync(Guid roomId, Guid userId, CancellationToken cancellationToken = default);

    Task<RoomParticipant?> FindParticipantOrDefaultAsync(Guid roomId, Guid userId, CancellationToken cancellationToken = default);

    Task<IPagedList<RoomPageDetail>> GetDetailedPageAsync(RoomPageDetailRequestFilter filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task<RoomDetail?> GetByIdAsync(Guid roomId, CancellationToken cancellationToken = default);
}
