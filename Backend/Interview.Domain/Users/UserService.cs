using CSharpFunctionalExtensions;
using Interview.Domain.Users.Roles;

namespace Interview.Domain.Users;

public sealed class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly AdminUsers _adminUsers;

    public UserService(IUserRepository userRepository, IRoleRepository roleRepository, AdminUsers adminUsers)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _adminUsers = adminUsers;
    }

    public async Task<Result<User?>> GetByIdentityAsync(Guid userIdentity, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindByIdAsync(userIdentity, cancellationToken);

        if (user == null)
        {
            return Result.Failure<User?>($"User not found by identity {userIdentity}");
        }

        return user;
    }

    public async Task<User> UpsertByTwitchIdentityAsync(User user, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository.FindByTwitchIdentityAsync(user.TwitchIdentity, cancellationToken);
        if (existingUser != null)
        {
            existingUser.Nickname = user.Nickname;
            await _userRepository.UpdateAsync(existingUser, cancellationToken);
            return existingUser;
        }

        var userRole = await GetUserRoleAsync(user.Nickname, cancellationToken);
        if (userRole == null)
        {
            throw new InvalidOperationException("Not found \"User\" role");
        }

        var insertUser = new User(user.Nickname, user.TwitchIdentity);
        insertUser.Roles.Add(userRole);
        await _userRepository.CreateAsync(insertUser, cancellationToken);
        return insertUser;
    }

    public Task<List<User>> GetByRoleAsync(RoleNameType roleNameType, CancellationToken cancellationToken = default)
    {
        var roleName = RoleName.FromValue((int)roleNameType);
        return _userRepository.GetByRoleAsync(roleName, cancellationToken);
    }

    private Task<Role?> GetUserRoleAsync(string nickname, CancellationToken cancellationToken)
    {
        var roleName = _adminUsers.IsAdmin(nickname) ? RoleName.Admin : RoleName.User;
        return _roleRepository.FindByIdAsync(roleName.Id, cancellationToken);
    }
}
