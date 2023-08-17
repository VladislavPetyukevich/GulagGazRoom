namespace Interview.Domain.Users;

public interface ICurrentUserAccessor
{
    Guid? UserId { get; }

    User? UserDetailed { get; }

    Guid GetUserIdOrThrow()
    {
        return UserId ?? throw new InvalidOperationException("Unable to find current user.");
    }

    bool HasUserId()
    {
        return UserId is not null && UserId != Guid.Empty;
    }

    User GetUserDetailOrThrow()
    {
        return UserDetailed ?? throw new InvalidOperationException("Unable to find current user.");
    }
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
