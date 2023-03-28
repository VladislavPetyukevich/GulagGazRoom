using Ardalis.SmartEnum;

namespace Interview.Domain.Users.Roles;

public enum RoleNameType
{
    /// <summary>
    /// User.
    /// </summary>
    User = 1,

    /// <summary>
    /// Admin.
    /// </summary>
    Admin,
}

public sealed class RoleName : SmartEnum<RoleName>
{
    public static readonly RoleName Unknown = new(Guid.Empty, "Unknown", 0);
    public static readonly RoleName User = new(Guid.Parse("ab45a82b-aa1c-11ed-abe8-f2b335a02ee9"), RoleNameConstants.User, (int)RoleNameType.User);
    public static readonly RoleName Admin = new(Guid.Parse("ab45cf57-aa1c-11ed-970f-98dc442de35a"), RoleNameConstants.Admin, (int)RoleNameType.Admin);

    private RoleName(Guid id, string name, int value)
        : base(name, value)
    {
        Id = id;
    }

    public Guid Id { get; }
}
