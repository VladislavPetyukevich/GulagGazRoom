using Interview.Domain.Rooms;
using Interview.Domain.Rooms.Service.Records.Response.Page;
using Interview.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

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

    public Task<IPagedList<RoomDetail>> GetDetailedPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return GetRoomDetailQueryable()
            .OrderBy(e => e.Id)
            .ToPagedListAsync(pageNumber, pageSize, cancellationToken);
    }

    public Task<RoomDetail?> GetByIdAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        return GetRoomDetailQueryable()
            .FirstOrDefaultAsync(room => room.Id == roomId, cancellationToken: cancellationToken);
    }

    private IQueryable<RoomDetail> GetRoomDetailQueryable()
    {
        return Set
            .Include(e => e.Users)
            .Include(e => e.Questions)
            .Select(e => new RoomDetail
            {
                Id = e.Id,
                Name = e.Name,
                Questions = e.Questions.Select(question =>
                    new RoomQuestionDetail { Id = question.Id, Value = question.Value, }).ToList(),
                Users = e.Users.Select(usr =>
                    new RoomUserDetail { Id = usr.Id, Nickname = usr.Nickname, }).ToList(),
            });
    }
}
