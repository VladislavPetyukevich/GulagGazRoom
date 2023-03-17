using Interview.Domain.Repository;
using Interview.Domain.Rooms.Service.Records.Response.Page;
using X.PagedList;

namespace Interview.Domain.Rooms;

public interface IRoomRepository : IRepository<Room>
{
    Task<bool> HasUserAsync(Guid roomId, Guid userId, CancellationToken cancellationToken = default);

    Task<IPagedList<RoomDetail>> GetDetailedPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    public Task<RoomDetail?> GetByIdAsync(Guid roomId, CancellationToken cancellationToken = default);

    public Task<bool> HasAnyQuestion(Guid roomId);
}
