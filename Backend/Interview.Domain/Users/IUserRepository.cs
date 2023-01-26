namespace Interview.Domain.Users;

public interface IUserRepository
{
    Task<User?> FindUserAsync(string nickname, CancellationToken cancellationToken = default);
}