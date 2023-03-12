using Interview.Domain.Rooms.Service.Records.Response.FindById;
using Interview.Domain.Rooms.Service.Records.Response.Page;
using X.PagedList;

namespace Interview.Domain.Rooms;

public interface IRoomRepository : IRepository<Room>
{
    Task<bool> HasUserAsync(Guid roomId, Guid userId, CancellationToken cancellationToken = default);

    Task<IPagedList<RoomPageDetail>> GetDetailedPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    public Task<RoomFoundItem?> GetByIdAsync(Guid roomId, CancellationToken cancellationToken = default);
}
