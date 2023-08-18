using Interview.Domain.Users.Roles;

namespace Interview.Domain.Users;

public interface ICurrentUserAccessor
{
    Guid? UserId { get; }

    User? UserDetailed { get; }

    bool HasId(Guid? id)
    {
        return UserId == id;
    }

    bool HasRole(RoleName roleName) =>
        UserDetailed is not null && UserDetailed.Roles.Exists(it => it.Name == roleName);

    bool IsAdmin() => HasRole(RoleName.Admin);
}

public interface IEditableCurrentUserAccessor : ICurrentUserAccessor
{
    void SetUser(User user);
}

public sealed class CurrentUserAccessor : IEditableCurrentUserAccessor
{
    private User? _currentUser;

    public Guid? UserId => _currentUser?.Id;

    public User? UserDetailed => _currentUser;

    public void SetUser(User user) => _currentUser = user;
}
