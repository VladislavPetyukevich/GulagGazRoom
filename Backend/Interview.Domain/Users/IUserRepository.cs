using Interview.Domain.Repository;

namespace Interview.Domain.Users;

public interface IUserRepository : IRepository<User>
{
    Task<User?> FindByNicknameAsync(string nickname, CancellationToken cancellationToken = default);

    Task<User?> FindByTwitchIdentityAsync(string twitchIdentity, CancellationToken cancellationToken = default);
}
