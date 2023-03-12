using Interview.Domain.Rooms;
using Interview.Domain.Rooms.Service.Records.Response.FindById;
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

    public Task<IPagedList<RoomPageDetail>> GetDetailedPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return Set
            .Include(e => e.Users)
            .Include(e => e.Questions)
            .OrderBy(e => e.Id)
            .Select(e => new RoomPageDetail
            {
                Id = e.Id,
                Name = e.Name,
                Questions = e.Questions.Select(question =>
                    new RoomQuestionPageDetail { Id = question.Id, Value = question.Value, }).ToList(),
                Users = e.Users.Select(usr =>
                    new RoomUserPageDetail { Id = usr.Id, Nickname = usr.Nickname, }).ToList(),
            })
            .ToPagedListAsync(pageNumber, pageSize, cancellationToken);
    }

    public async Task<RoomFoundItem?> GetByIdAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        var foundRoom = await ApplyIncludes(Set)
            .Include(e => e.Users)
            .Include(e => e.Questions)
            .FirstOrDefaultAsync(room => room.Id == roomId, cancellationToken: cancellationToken);

        if (foundRoom == null)
        {
            return null;
        }

        return new RoomFoundItem
        {
            Id = foundRoom.Id,
            Name = foundRoom.Name,
            Questions =
                foundRoom.Questions.Select(question =>
                    new RoomQuestionFoundItem { Id = question.Id, Value = question.Value, }).ToList(),
            Users = foundRoom.Users
                .Select(user => new RoomUserFoundItem { Id = user.Id, Nickname = user.Nickname, }).ToList(),
        };
    }
}
