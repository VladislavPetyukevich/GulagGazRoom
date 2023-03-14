using Interview.Domain.RoomUsers;
using Interview.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Interview.Infrastructure.RoomParticipants;

public class RoomParticipantRepository : EfRepository<RoomParticipant>, IRoomParticipantRepository
{
    public RoomParticipantRepository(AppDbContext db)
        : base(db)
    {
    }

    protected override IQueryable<RoomParticipant> ApplyIncludes(DbSet<RoomParticipant> set) => Set;
}
