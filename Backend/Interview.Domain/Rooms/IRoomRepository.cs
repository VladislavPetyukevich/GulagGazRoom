namespace Interview.Domain.Rooms;

public interface IRoomRepository : IRepository<Room>
{
    Task<bool> HasUserAsync(Guid roomId, Guid userId, CancellationToken cancellationToken = default);
}
