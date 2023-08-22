using Interview.Domain;
using Interview.Domain.Repository;
using Interview.Domain.Users;
using Interview.Domain.Users.Permissions;

namespace Interview.Infrastructure.Database.Processors;

public class AccessEntityActionPreProcessor : AbstractPreProcessor
{
    private readonly ICurrentUserAccessor _currentUserAccessor;

    private readonly ICurrentPermissionAccessor _currentPermissionAccessor;

    public AccessEntityActionPreProcessor(
        ICurrentUserAccessor currentUserAccessor,
        ICurrentPermissionAccessor currentPermissionAccessor)
    {
        _currentUserAccessor = currentUserAccessor;
        _currentPermissionAccessor = currentPermissionAccessor;
    }

    protected override void AddEntityHandler(Entity entity, CancellationToken cancellationToken = default)
    {
        PermissionHandler(entity, PermissionNameType.Write);
    }

    protected override void ModifyOriginalEntityHandler(
        Entity entity, CancellationToken cancellationToken = default)
    {
        PermissionHandler(entity, PermissionNameType.Modify);
    }

    private void PermissionHandler(Entity entity, PermissionNameType permissionNameType)
    {
        if (HasPermission(entity, permissionNameType))
        {
            return;
        }

        throw new AccessDeniedException(
            ExceptionMessage.ResourceAccessDined(entity.GetType().Name, permissionNameType));
    }

    private bool HasPermission(Entity entity, PermissionNameType permissionNameType)
    {
        var isUserAdmin = _currentUserAccessor.IsAdmin();

        if (isUserAdmin)
        {
            return true;
        }

        if (permissionNameType == PermissionNameType.Modify)
        {
            var isUserOwner = _currentUserAccessor.HasId(entity.CreatedById);

            if (isUserOwner)
            {
                return true;
            }
        }

        var permissionName = entity.GetType().Name;

        var isProtectedResource = _currentPermissionAccessor.IsProtectedResource(permissionName);

        if (isProtectedResource is not true)
        {
            return true;
        }

        var hasPermission = _currentUserAccessor.HasPermission(permissionName, permissionNameType);

        return hasPermission;
    }
}
