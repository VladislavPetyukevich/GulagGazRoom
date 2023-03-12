using FluentAssertions;
using Interview.Domain.Users;
using Interview.Domain.Users.Roles;
using Interview.Infrastructure.Users;
using Microsoft.EntityFrameworkCore;

namespace Interview.Test.Integrations;

public class AppDbContextTest
{
    [Fact]
    public async Task DbContext_Should_Update_Create_And_Update_Dates()
    {
        var clock = new TestSystemClock();
        await using var appDbContext = new TestAppDbContextFactory().Create(clock);

        var userService = new UserService(new UserRepository(appDbContext), new RoleRepository(appDbContext), new AdminUsers());

        var user = new User("Dima", "dima@yandex.ru", "1");
        user.UpdateCreateDate(clock.UtcNow.DateTime);
        await userService.UpsertByTwitchIdentityAsync(user);

        user.Id.Should().NotBe(Guid.Empty);

        var savedUser = new User(user.Id, "Dima", "dima@yandex.ru", "1");
        savedUser.UpdateCreateDate(clock.UtcNow.DateTime);
        var role = new Role(RoleName.User);
        role.UpdateCreateDate(clock.UtcNow.DateTime);
        savedUser.Roles.Add(role);

        user.Should().BeEquivalentTo(savedUser);
    }
}
