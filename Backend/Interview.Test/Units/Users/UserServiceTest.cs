using FluentAssertions;
using Interview.Domain.Users;
using Interview.Domain.Users.Roles;
using Moq;

namespace Interview.Test.Units.Users;

public class UserServiceTest
{

    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IRoleRepository> _mockRoleRepository;

    private readonly UserService _userService;

    public UserServiceTest()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockRoleRepository = new Mock<IRoleRepository>();

        _userService = new UserService(_mockUserRepository.Object, _mockRoleRepository.Object, new AdminUsers());
    }

    [Fact]
    public async Task UpsertUsersWhenUserNotExistsInDatabaseAndRoleNotFound()
    {
        var user = new User("Dima", "1");

        _mockUserRepository.Setup(repository =>
                repository.FindByTwitchIdentityAsync(user.TwitchIdentity, default))
            .Returns(() => Task.FromResult<User>(null));

        _mockRoleRepository.Setup(repository =>
                repository.FindByIdAsync(RoleName.User.Id, default))
            .Returns(() => ValueTask.FromResult<Role>(null));

        var throwsAsync = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _userService.UpsertByTwitchIdentityAsync(user));

        throwsAsync.Message.Should().NotBeNull().And.NotBeEmpty();

        _mockUserRepository.Verify(repository =>
            repository.FindByTwitchIdentityAsync(user.TwitchIdentity, default), Times.Once);
        _mockRoleRepository.Verify(repository =>
            repository.FindByIdAsync(RoleName.User.Id, default), Times.Once);
        _mockUserRepository.Verify(repository =>
            repository.CreateAsync(It.IsAny<User>(), default), Times.Never);
    }

}
