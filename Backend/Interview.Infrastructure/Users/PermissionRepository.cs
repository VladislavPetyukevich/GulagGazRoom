using Interview.Domain.Permissions;
using Interview.Domain.Users.Permissions;
using Interview.Infrastructure.Database;
using X.PagedList;

namespace Interview.Infrastructure.Users;

public class PermissionRepository : EfRepository<Permission>, IPermissionRepository
{
    public PermissionRepository(AppDbContext db)
        : base(db)
    {
    }

    public Task<List<Permission>> FindAllByTypes(HashSet<SEPermission> types, CancellationToken cancellationToken = default)
    {
        return Set.Where(it => types.Contains(it.Type)).ToListAsync(cancellationToken);
    }
}
