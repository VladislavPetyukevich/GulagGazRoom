using Entity = Interview.Domain.Repository.Entity;

namespace Interview.Domain.Users.Permissions;

public class Permission : Entity
{
    public Permission(Guid id, PermissionNameType type, string resource)
        : base(id)
    {
        Type = type;
        Resource = resource;
    }

    public PermissionNameType Type { get; set; }

    public string Resource { get; set; }
}
