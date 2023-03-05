using Interview.Domain.Users.Roles;

namespace Interview.Domain.Users;

public sealed class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
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

        var userRole = await _roleRepository.FindByIdAsync(RoleName.User.Id, cancellationToken);
        if (userRole == null)
        {
            throw new InvalidOperationException("Not found \"User\" role");
        }

        user.Roles.Add(userRole);
        await _userRepository.CreateAsync(user, cancellationToken);
    }
}
