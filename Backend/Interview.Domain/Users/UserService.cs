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

    public async Task UpsertByTwitchIdentityAsync(User user, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository.FindByTwitchIdentityAsync(user.TwitchIdentity, cancellationToken);
        if (existingUser != null)
        {
            existingUser.Nickname = user.Nickname;
            existingUser.Email = user.Email;
            user.Roles.AddRange(existingUser.Roles);
            user.Id = existingUser.Id;
            await _userRepository.UpdateAsync(existingUser, cancellationToken);
            return;
        }

        var userRole = await GetUserRoleAsync(user.Nickname, cancellationToken);
        if (userRole == null)
        {
            throw new InvalidOperationException("Not found \"User\" role");
        }

        user.Roles.Add(userRole);
        await _userRepository.CreateAsync(user, cancellationToken);
    }

    private ValueTask<Role?> GetUserRoleAsync(string nickname, CancellationToken cancellationToken)
    {
        var roleName = _adminUsers.IsAdmin(nickname) ? RoleName.Admin : RoleName.User;
        return _roleRepository.FindByIdAsync(roleName.Id, cancellationToken);
    }
}
