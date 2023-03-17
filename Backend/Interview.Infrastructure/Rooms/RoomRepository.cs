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
        return Set
            .Include(room => room.Participants)
            .AnyAsync(
                room => room.Id == roomId && room.Participants.Any(participant => participant.User.Id == userId),
                cancellationToken);
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

    public Task<bool> HasAnyQuestion(Guid roomId)
    {
        return Set
            .Include(room => room.Questions)
            .AnyAsync(predicate: room => room.Id == roomId && room.Questions.Count != 0);
    }

    protected override IQueryable<Room> ApplyIncludes(DbSet<Room> set)
        => Set.Include(e => e.Participants)
            .Include(e => e.Questions);

    private IQueryable<RoomDetail> GetRoomDetailQueryable()
    {
        return Set
            .Include(e => e.Participants)
            .Include(e => e.Questions)
            .Select(e => new RoomDetail
            {
                Id = e.Id,
                Name = e.Name,
                Questions = e.Questions.Select(question => question.Question)
                    .Select(question => new RoomQuestionDetail { Id = question.Id, Value = question.Value, })
                    .ToList(),
                Users = e.Participants.Select(participant =>
                        new RoomUserDetail { Id = participant.User.Id, Nickname = participant.User.Nickname, })
                    .ToList(),
            });
    }
}
