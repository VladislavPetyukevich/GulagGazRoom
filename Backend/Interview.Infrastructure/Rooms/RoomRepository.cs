using Interview.Domain.Rooms;
using Interview.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using NSpecifications;
using X.PagedList;

namespace Interview.Infrastructure.Rooms;

public class RoomRepository : EfRepository<Room>, IRoomRepository
{
    public RoomRepository(AppDbContext db)
        : base(db)
    {
    }

    public new Task<IPagedList<Room>> GetPage(ISpecification<Room> specification, int pageNumber, int pageSize)
    {
        return Set
            .Include(room => room.Users)
            .Include(room => room.Questions)
            .Where(specification.Expression)
            .ToPagedListAsync(pageNumber, pageSize);
    }

    public new Task<IPagedList<Room>> GetPage(int pageNumber, int pageSize)
    {
        return Set
            .Include(room => room.Users)
            .Include(room => room.Questions)
            .OrderBy(entity => entity.Id)
            .ToPagedListAsync(pageNumber, pageSize);
    }

    public Task<bool> HasUserAsync(Guid roomId, Guid userId, CancellationToken cancellationToken = default)
    {
        return Set.Include(e => e.Users)
            .AnyAsync(e => e.Id == roomId && e.Users.Any(user => user.Id == userId), cancellationToken);
    }
}
