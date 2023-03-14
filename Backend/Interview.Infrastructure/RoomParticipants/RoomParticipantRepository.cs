using Interview.Domain.RoomParticipants;
using Interview.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Interview.Infrastructure.RoomParticipants;

public class RoomParticipantRepository : EfRepository<RoomParticipant>, IRoomParticipantRepository
{
    public RoomParticipantRepository(AppDbContext db)
        : base(db)
    {
    }

    public Task<bool> FindByRoomIdAndUserId(Guid roomId, Guid userId) =>
        ApplyIncludes(Set).AnyAsync(participant => participant.Room.Id == roomId && participant.User.Id == userId);

    protected override IQueryable<RoomParticipant> ApplyIncludes(DbSet<RoomParticipant> set) => set
            .Include(participant => participant.Room)
            .Include(participant => participant.User);
}
