using FluentAssertions;
using Interview.Domain.Users;
using Interview.Infrastructure.Users;
using Microsoft.EntityFrameworkCore;

namespace Interview.Test.Integrations;

public class UserServiceTest
{
    [Fact]
    public async Task UpsertUsersWhenUserExistsInDatabase()
    {
        var testSystemClock = new TestSystemClock();
        await using var appDbContext = new TestAppDbContextFactory().Create(testSystemClock);

        var entity = new User("Ivan", "ivan@yandex.ru", "1");

        appDbContext.Users.Add(entity);

        await appDbContext.SaveChangesAsync();

        var userService = new UserService(new UserRepository(appDbContext), new RoleRepository(appDbContext), new AdminUsers());

        var user = new User("Dima", "dima@yandex.ru", "1");
        user.UpdateCreateDate(testSystemClock.UtcNow.Date);
        await userService.UpsertByTwitchIdentityAsync(user);

        var savedUser = await appDbContext.Users.SingleAsync();

        user.Should().BeEquivalentTo(savedUser);
    }

    [Fact]
    public async Task UpsertUsersWhenUserNotExistsInDatabase()
    {
        var testSystemClock = new TestSystemClock();
        await using var appDbContext = new TestAppDbContextFactory().Create(testSystemClock);

        appDbContext.Users.Count().Should().Be(0);

        var userService = new UserService(new UserRepository(appDbContext), new RoleRepository(appDbContext), new AdminUsers());

        var user = new User("Dima", "dima@yandex.ru", "1");
        user.UpdateCreateDate(testSystemClock.UtcNow.Date);

        await userService.UpsertByTwitchIdentityAsync(user);

        var savedUser = await appDbContext.Users.SingleAsync();

        user.Should().BeEquivalentTo(savedUser);
    }

}
