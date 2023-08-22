using Interview.Domain.Users;
using Interview.Domain.Users.Permissions;
using Interview.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Interview.Infrastructure.Users;

public sealed class CurrentPermissionAccessor : ICurrentPermissionAccessor
{
    private readonly Lazy<List<Permission>> _lazyPermission;

    public CurrentPermissionAccessor(AppDbContext appDbContext)
    {
        _lazyPermission = new Lazy<List<Permission>>(() =>
            appDbContext.Permissions.AsNoTracking().ToList());
    }

    public bool IsProtectedResource(string resource)
    {
        var protectedResources = _lazyPermission.Value
            .Select(permission => permission.Resource.ToLower())
            .ToHashSet();

        return protectedResources.Contains(resource.ToLower());
    }
}
