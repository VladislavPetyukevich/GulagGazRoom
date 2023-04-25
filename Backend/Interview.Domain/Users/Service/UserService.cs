using CSharpFunctionalExtensions;
using Interview.Domain.Repository;
using Interview.Domain.ServiceResults.Errors;
using Interview.Domain.ServiceResults.Success;
using Interview.Domain.Users.Records;
using Interview.Domain.Users.Roles;
using NSpecifications;
using X.PagedList;

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

    public Task<IPagedList<UserDetail>> FindPageAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var mapperUserDetail = new Mapper<User, UserDetail>(user => new UserDetail
        {
            Id = user.Id,
            Nickname = user.Nickname,
            Avatar = user.Avatar,
            Roles = user.Roles.Select(role => role.Name.Name).ToList(),
            TwitchIdentity = user.TwitchIdentity,
        });

        return _userRepository.GetPageDetailedAsync(mapperUserDetail, pageNumber, pageSize, cancellationToken);
    }

    public async Task<Result<ServiceResult<UserDetail>, ServiceError>> FindByNicknameAsync(
        string nickname, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindByNicknameAsync(nickname, cancellationToken);

        if (user == null)
        {
            return ServiceError.NotFound($"Not found user with nickname [{nickname}]");
        }

        return ServiceResult.Ok(new UserDetail
        {
            Id = user.Id,
            Avatar = user.Avatar,
            Nickname = user.Nickname,
            Roles = user.Roles.Select(role => role.Name.Name).ToList(),
            TwitchIdentity = user.TwitchIdentity,
        });
    }

    public async Task<Result<User?>> GetByIdentityAsync(
        Guid userIdentity, CancellationToken cancellationToken = default)
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
            existingUser.Avatar = user.Avatar;
            await _userRepository.UpdateAsync(existingUser, cancellationToken);
            return existingUser;
        }

        var userRole = await GetUserRoleAsync(user.Nickname, cancellationToken);
        if (userRole == null)
        {
            throw new InvalidOperationException("Not found \"User\" role");
        }

        var insertUser = new User(user.Nickname, user.TwitchIdentity) { Avatar = user.Avatar };

        insertUser.Roles.Add(userRole);
        await _userRepository.CreateAsync(insertUser, cancellationToken);
        return insertUser;
    }

    public Task<IPagedList<UserDetail>> FindByRoleAsync(int pageNumber, int pageSize, RoleNameType roleNameType, CancellationToken cancellationToken = default)
    {
        var roleName = RoleName.FromValue((int)roleNameType);

        var spec = new Spec<User>(user => user.Roles.Any(r => r.Name == roleName));
        var mapper = new Mapper<User, UserDetail>(user => new UserDetail
        {
            Id = user.Id,
            Nickname = user.Nickname,
            Avatar = user.Avatar,
            Roles = user.Roles.Select(role => role.Name.Name).ToList(),
            TwitchIdentity = user.TwitchIdentity,
        });

        return _userRepository.GetPageAsync(spec, mapper, pageNumber, pageSize, cancellationToken);
    }

    private Task<Role?> GetUserRoleAsync(string nickname, CancellationToken cancellationToken)
    {
        var roleName = _adminUsers.IsAdmin(nickname) ? RoleName.Admin : RoleName.User;

        return _roleRepository.FindByIdAsync(roleName.Id, cancellationToken);
    }
}
