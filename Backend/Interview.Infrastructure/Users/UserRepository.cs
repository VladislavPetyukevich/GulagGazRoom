using Interview.Domain.Repository;
using Interview.Domain.Users;
using Interview.Domain.Users.Roles;
using Interview.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Interview.Infrastructure.Users;

public class UserRepository : EfRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext db)
        : base(db)
    {
    }

    public Task<User?> FindByNicknameAsync(string nickname, CancellationToken cancellationToken = default)
    {
        return ApplyDetailed(Set)
            .FirstOrDefaultAsync(user => user.Nickname == nickname, cancellationToken);
    }

    public Task<List<User>> GetByRoleAsync(RoleName roleName, CancellationToken cancellationToken = default)
    {
        return ApplyDetailed(Set)
            .Where(e => e.Roles.Any(r => r.Name == roleName))
            .ToListAsync(cancellationToken);
    }

    public Task<IPagedList<User>> FindPageByRoleAsync<TRes>(
        IMapper<User, TRes> mapper,
        int pageNumber,
        int pageSize,
        RoleName roleName,
        CancellationToken cancellationToken = default)
    {
        return ApplyDetailed(Set)
            .Where(e => e.Roles.Any(r => r.Name == roleName))
            .ToPagedListAsync(pageNumber, pageSize, cancellationToken);
    }

    public Task<User?> FindByTwitchIdentityAsync(string twitchIdentity, CancellationToken cancellationToken = default)
    {
        return ApplyDetailed(Set)
            .FirstOrDefaultAsync(user => user.TwitchIdentity == twitchIdentity, cancellationToken);
    }

    protected override IQueryable<User> ApplyDetailed(DbSet<User> set)
        => set.Include(e => e.Roles);
}
