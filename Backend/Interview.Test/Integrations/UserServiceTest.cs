using FluentAssertions;
using Interview.Domain.Users;
using Interview.Infrastructure.Users;
using Microsoft.EntityFrameworkCore;

namespace Interview.Test.Integrations;

public class UserServiceTest
{
    [Fact(DisplayName = "'UpsertByTwitchIdentityAsync' when there is already such a user in the database")]
    public async Task UpsertUsersWhenUserExistsInDatabase()
    {
        var clock = new TestSystemClock();
        await using var appDbContext = new TestAppDbContextFactory().Create(clock);
        var entity = new User("Ivan", "1");
        appDbContext.Users.Add(entity);
        await appDbContext.SaveChangesAsync();

        var userService = new UserService(new UserRepository(appDbContext), new RoleRepository(appDbContext), new AdminUsers());
        var user = new User("Dima", "1");
        var upsertUser = await userService.UpsertByTwitchIdentityAsync(user);

        var expectedUser = new User(entity.Id, user.Nickname, user.TwitchIdentity);
        expectedUser.UpdateCreateDate(clock.UtcNow.DateTime);
        expectedUser.Roles.AddRange(entity.Roles);
        upsertUser.Should().BeEquivalentTo(expectedUser);
    }

    [Fact(DisplayName = "'UpsertByTwitchIdentityAsync' when there is no such user in the database")]
    public async Task UpsertUsersWhenUserNotExistsInDatabase()
    {
        var clock = new TestSystemClock();
        await using var appDbContext = new TestAppDbContextFactory().Create(clock);
        appDbContext.Users.Count().Should().Be(0);
        var userService = new UserService(new UserRepository(appDbContext), new RoleRepository(appDbContext), new AdminUsers());
        var user = new User("Dima", "1");

        var upsertUser = await userService.UpsertByTwitchIdentityAsync(user);

        var savedUser = await appDbContext.Users.SingleAsync();
        upsertUser.Should().BeEquivalentTo(savedUser);
    }

    [Fact(DisplayName = "Inserting a user if there are no roles in the database")]
    public async Task UpsertUsersWhenDbNotContainRoles()
    {
        var clock = new TestSystemClock();
        await using var appDbContext = new TestAppDbContextFactory().Create(clock);
        appDbContext.Roles.RemoveRange(appDbContext.Roles);
        await appDbContext.SaveChangesAsync();
        appDbContext.Users.Count().Should().Be(0);
        var userService = new UserService(new UserRepository(appDbContext), new RoleRepository(appDbContext), new AdminUsers());
        var user = new User("Dima", "1");

        var error = await Assert.ThrowsAsync<InvalidOperationException>(async () => await userService.UpsertByTwitchIdentityAsync(user));

        error.Message.Should().NotBeNull().And.NotBeEmpty();
    }
}
