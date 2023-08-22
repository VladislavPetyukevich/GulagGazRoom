using Interview.Domain.Users.Permissions;

namespace Interview.Domain.Users.Records;

public class PermissionDetail
{
    public Guid Id { get; init; }

    public PermissionNameType Type { get; init; }

    public string Resource { get; init; }
}
