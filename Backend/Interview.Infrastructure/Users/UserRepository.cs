using Interview.Domain.Users;
using Interview.Domain.Users.Roles;
using Interview.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Interview.Infrastructure.Users;

public class UserRepository : EfRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext db)
        : base(db)
    {
    }

    public Task<User?> FindByNicknameAsync(string nickname, CancellationToken cancellationToken = default)
    {
        return Set.Include(e => e.Roles)
            .FirstOrDefaultAsync(user => user.Nickname == nickname, cancellationToken);
    }

    public Task<List<User>> GetByRoleAsync(RoleName roleName, CancellationToken cancellationToken = default)
    {
        return Set.Include(e => e.Roles)
            .Where(e => e.Roles.Any(r => r.Name == roleName))
            .ToListAsync(cancellationToken);
    }

    public Task<User?> FindByTwitchIdentityAsync(string twitchIdentity, CancellationToken cancellationToken = default)
    {
        return Set.Include(e => e.Roles)
            .FirstOrDefaultAsync(user => user.TwitchIdentity == twitchIdentity, cancellationToken);
    }

    protected override IQueryable<User> ApplyIncludes(DbSet<User> set)
        => set.Include(e => e.Roles);
}
