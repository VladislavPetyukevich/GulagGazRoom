using Interview.Domain.RoomReviews;
using Interview.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Interview.Infrastructure.RoomReviews;

public class RoomReviewRepository : EfRepository<RoomReview>, IRoomReviewRepository
{
    public RoomReviewRepository(AppDbContext db)
        : base(db)
    {
    }

    protected override IQueryable<RoomReview> ApplyIncludes(DbSet<RoomReview> set)
    {
        return set
            .Include(it => it.Room)
            .Include(it => it.User);
    }
}
