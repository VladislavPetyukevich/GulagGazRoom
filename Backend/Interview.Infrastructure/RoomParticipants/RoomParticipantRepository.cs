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

    public Task<RoomParticipant?> FindByRoomIdAndUserId(
        Guid roomId, Guid userId, CancellationToken cancellationToken = default)
    {
        return ApplyDetailed(Set)
            .Where(participant => participant.Room.Id == roomId && participant.User.Id == userId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> IsExistsByRoomIdAndUserIdAsync(
        Guid roomId, Guid userId, CancellationToken cancellationToken = default)
    {
        return ApplyDetailed(Set)
            .AnyAsync(participant => participant.Room.Id == roomId && participant.User.Id == userId, cancellationToken);
    }

    protected override IQueryable<RoomParticipant> ApplyDetailed(DbSet<RoomParticipant> set) => set
        .Include(participant => participant.Room)
        .Include(participant => participant.User);
}
