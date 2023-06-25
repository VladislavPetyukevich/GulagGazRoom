using Interview.Domain.RoomReviews;
using Interview.Domain.Rooms.Service.Records.Response.Detail;
using Interview.Domain.Rooms.Service.Records.Response.Page;
using Interview.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Interview.Infrastructure.RoomReviews;

public class RoomReviewRepository : EfRepository<RoomReview>, IRoomReviewRepository
{
    public RoomReviewRepository(AppDbContext db)
        : base(db)
    {
    }

    public Task<IPagedList<RoomReviewPageDetail>> GetDetailedPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return Set
            .Include(e => e.Room)
            .Include(e => e.User)
            .Select(e => new RoomReviewPageDetail
            {
                Id = e.Id,
                RoomId = e.Room!.Id,
                User = new RoomUserDetail { Id = e.User!.Id, Nickname = e.User.Nickname },
                Review = e.Review,
                State = e.SeRoomReviewState.EnumValue,
            })
            .OrderBy(e => e.Id)
            .ToPagedListAsync(pageNumber, pageSize, cancellationToken);
    }

    protected override IQueryable<RoomReview> ApplyIncludes(DbSet<RoomReview> set)
    {
        return set
            .Include(it => it.Room)
            .Include(it => it.User);
    }
}
