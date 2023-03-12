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
        await using var appDbContext = new TestAppDbContextFactory().Create();
        var entity = new User("Ivan", "1");
        appDbContext.Users.Add(entity);
        await appDbContext.SaveChangesAsync();

        var userService = new UserService(new UserRepository(appDbContext), new RoleRepository(appDbContext), new AdminUsers());
        var user = new User("Dima", "1");
        var upsertUser = await userService.UpsertByTwitchIdentityAsync(user);

        var savedUser = await appDbContext.Users.SingleAsync();
        upsertUser.Should().BeEquivalentTo(savedUser);
    }

    [Fact]
    public async Task UpsertUsersWhenUserNotExistsInDatabase()
    {
        await using var appDbContext = new TestAppDbContextFactory().Create();
        appDbContext.Users.Count().Should().Be(0);
        var userService = new UserService(new UserRepository(appDbContext), new RoleRepository(appDbContext), new AdminUsers());
        var user = new User("Dima", "1");

        var upsertUser = await userService.UpsertByTwitchIdentityAsync(user);

        var savedUser = await appDbContext.Users.SingleAsync();
        upsertUser.Should().BeEquivalentTo(savedUser);
    }

    [Fact]
    public async Task UpsertUsersWhenDbNotContainRoles()
    {
        await using var appDbContext = new TestAppDbContextFactory().Create();
        appDbContext.Roles.RemoveRange(appDbContext.Roles);
        await appDbContext.SaveChangesAsync();
        appDbContext.Users.Count().Should().Be(0);
        var userService = new UserService(new UserRepository(appDbContext), new RoleRepository(appDbContext), new AdminUsers());
        var user = new User("Dima", "1");

        var error = await Assert.ThrowsAsync<InvalidOperationException>(async () => await userService.UpsertByTwitchIdentityAsync(user));

        error.Message.Should().NotBeNull().And.NotBeEmpty();
    }
}
