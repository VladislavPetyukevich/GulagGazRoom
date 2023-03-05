using Interview.Domain.Rooms;
using Interview.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Interview.Infrastructure.Rooms;

public class RoomRepository : EfRepository<Room>, IRoomRepository
{
    public RoomRepository(AppDbContext db)
        : base(db)
    {
    }

    public Task<bool> HasUserAsync(Guid roomId, Guid userId, CancellationToken cancellationToken = default)
    {
        return Set.Include(e => e.Users)
            .AnyAsync(e => e.Id == roomId && e.Users.Any(user => user.Id == userId), cancellationToken);
    }
}
