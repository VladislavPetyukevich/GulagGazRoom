using FluentAssertions;
using Interview.Domain.Users;
using Interview.Infrastructure.Users;
using Microsoft.EntityFrameworkCore;

namespace Interview.Test.Integrations;

public partial class UserServiceTest : IClassFixture<AppDbContextFixture>
{
    private readonly AppDbContextFixture _contextFixture;

    public UserServiceTest(AppDbContextFixture appDbContext)
    {
        _contextFixture = appDbContext;
    }

    // [Fact]
    public async Task UpsertUsersWhenUserExistsInDatabase()
    {
        var appDbContext = _contextFixture.Context;

        var entity = new User("Ivan", "ivan@yandex.ru", "1");

        appDbContext.Users.Add(entity);

        await appDbContext.SaveChangesAsync();

        var userService = new UserService(new UserRepository(appDbContext), new RoleRepository(appDbContext));

        var user = new User("Dima", "dima@yandex.ru", "1");

        await userService.UpsertByTwitchIdentityAsync(user);

        var savedUser = await appDbContext.Users.SingleAsync();

        user.Should().BeEquivalentTo(savedUser);
    }

    // [Fact]
    public async Task UpsertUsersWhenUserNotExistsInDatabase()
    {
        var appDbContext = _contextFixture.Context;

        appDbContext.Users.Count().Should().Be(0);

        var userService = new UserService(new UserRepository(appDbContext), new RoleRepository(appDbContext));

        var user = new User("Dima", "dima@yandex.ru", "1");

        await userService.UpsertByTwitchIdentityAsync(user);

        var savedUser = await appDbContext.Users.SingleAsync();

        user.Should().BeEquivalentTo(savedUser);
    }

}
