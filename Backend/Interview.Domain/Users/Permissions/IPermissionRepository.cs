using Interview.Domain.Permissions;
using Interview.Domain.Repository;

namespace Interview.Domain.Users.Permissions;

public interface IPermissionRepository : IRepository<Permission>
{
    public Task<List<Permission>> FindAllByTypes(HashSet<SEPermission> types, CancellationToken cancellationToken = default);
}
