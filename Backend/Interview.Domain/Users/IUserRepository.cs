using Interview.Domain.Repository;
using Interview.Domain.Users.Roles;

namespace Interview.Domain.Users;

public interface IUserRepository : IRepository<User>
{
    Task<User?> FindByNicknameAsync(string nickname, CancellationToken cancellationToken = default);

    Task<List<User>> GetByRoleAsync(RoleName roleName, CancellationToken cancellationToken = default);

    Task<User?> FindByTwitchIdentityAsync(string twitchIdentity, CancellationToken cancellationToken = default);
}
