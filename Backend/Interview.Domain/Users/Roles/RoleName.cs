using Ardalis.SmartEnum;

namespace Interview.Domain.Users.Roles;

public sealed class RoleName : SmartEnum<RoleName>
{
    public static readonly RoleName Unknown = new RoleName( Guid.Empty, "Unknown", 0);
    public static readonly RoleName User = new RoleName(Guid.Parse("ab45a82b-aa1c-11ed-abe8-f2b335a02ee9"), RoleNameConstants.User, 1);
    public static readonly RoleName Admin = new RoleName(Guid.Parse("ab45cf57-aa1c-11ed-970f-98dc442de35a"), RoleNameConstants.Admin, 2);
    
    private RoleName(Guid id, string name, int value) : base(name, value)
    {
        Id = id;
    }
    
    public Guid Id { get; }
}