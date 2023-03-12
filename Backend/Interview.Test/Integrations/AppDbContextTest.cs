using FluentAssertions;
using Interview.Domain.Users;
using Interview.Domain.Users.Roles;
using Interview.Infrastructure.Users;
using Microsoft.EntityFrameworkCore;

namespace Interview.Test.Integrations;

public class AppDbContextTest
{
    [Fact(DisplayName = "AppDbContext should update the update date and the entity creation date when saving")]
    public async Task DbContext_Should_Update_Create_And_Update_Dates()
    {
        var clock = new TestSystemClock();
        await using var appDbContext = new TestAppDbContextFactory().Create(clock);

        var userService = new UserService(new UserRepository(appDbContext), new RoleRepository(appDbContext), new AdminUsers());

        var user = new User("Dima", "1");
        user.UpdateCreateDate(clock.UtcNow.DateTime);
        var upsertUser = await userService.UpsertByTwitchIdentityAsync(user);

        upsertUser.Id.Should().NotBe(Guid.Empty);

        var savedUser = new User(upsertUser.Id, "Dima", "1");
        savedUser.UpdateCreateDate(clock.UtcNow.DateTime);
        var role = new Role(RoleName.User);
        role.UpdateCreateDate(clock.UtcNow.DateTime);
        savedUser.Roles.Add(role);

        upsertUser.Should().BeEquivalentTo(savedUser);
    }
}
