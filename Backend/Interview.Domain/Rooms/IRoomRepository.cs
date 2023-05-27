using Interview.Domain.Repository;
using Interview.Domain.Rooms.Service.Records.Request;
using Interview.Domain.Rooms.Service.Records.Response.Detail;
using Interview.Domain.Rooms.Service.Records.Response.Page;
using X.PagedList;

namespace Interview.Domain.Rooms;

public interface IRoomRepository : IRepository<Room>
{
    Task<Analytics?> GetAnalyticsAsync(RoomAnalyticsRequest request, CancellationToken cancellationToken = default);

    Task<bool> HasUserAsync(Guid roomId, Guid userId, CancellationToken cancellationToken = default);

    Task<IPagedList<RoomPageDetail>> GetDetailedPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task<RoomDetail?> GetByIdAsync(Guid roomId, CancellationToken cancellationToken = default);
}
