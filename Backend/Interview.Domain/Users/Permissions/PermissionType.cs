using Ardalis.SmartEnum;

namespace Interview.Domain.Users.Permissions;

public enum PermissionType
{
#pragma warning disable SA1602
    Modify = 1,

    Write = 2,
#pragma warning restore SA1602
}

public sealed class PermissionNameType : SmartEnum<PermissionNameType>
{
    public static readonly PermissionNameType Modify = new(Guid.Parse("f642855a-44c0-4842-b259-8f15016e224d"), "modify", (int)PermissionType.Modify);
    public static readonly PermissionNameType Write = new(Guid.Parse("2385d5f3-eae4-4d54-89df-fdef5e552c0d"), "write", (int)PermissionType.Write);

    public PermissionNameType(Guid id, string name, int value)
        : base(name, value)
    {
        Id = id;
    }

    public Guid Id { get; }
}
