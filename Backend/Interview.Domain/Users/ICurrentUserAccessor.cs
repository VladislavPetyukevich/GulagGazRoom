namespace Interview.Domain.Users;

public interface ICurrentUserAccessor
{
    Guid UserId { get; }

    User UserDetailed { get; }
}

public interface IEditableCurrentUserAccessor : ICurrentUserAccessor
{
    void SetUser(User user);
}

public sealed class CurrentUserAccessor : IEditableCurrentUserAccessor
{
    private User? _currentUser;

    public Guid UserId => _currentUser?.Id ?? throw NotFoundUserException;

    public User UserDetailed => _currentUser ?? throw NotFoundUserException;

    private static InvalidOperationException NotFoundUserException => new("Unable to find current user.");

    public void SetUser(User user) => _currentUser = user;
}
