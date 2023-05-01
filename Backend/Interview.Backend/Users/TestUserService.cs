using System.Net;
using System.Security.Claims;
using Interview.Backend.Auth;
using Interview.Backend.Responses;
using Interview.Infrastructure.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace Interview.Backend.Users;

public class TestUserService
{
    private readonly TestUser[] _users = new[]
    {
        new TestUser(
            Guid.Parse("b5a05f34-e44d-11ed-b49f-e8e34e3377ec"),
            "TEST_BACKEND_DEV_USER",
            "d1731c50-e44d-11ed-905c-d08c09609150",
            RoleName.User),
        new TestUser(
            Guid.Parse("7d988da5-e5df-11ed-a5cd-d942d21eb67c"),
            "TEST_BACKEND_DEV_ADMIN",
            "7d988da5-e5df-11ed-a5cd-885a9498c853",
            RoleName.Admin),
    };

    public Task AddUsersAsync(AppDbContext db)
    {
        var addAny = false;
        foreach (var testUser in _users)
        {
            if (db.Users.Any(e => e.Id == testUser.Id))
            {
                continue;
            }

            db.Users.Add(new User(testUser.Nickname, testUser.TwitchIdentity)
            {
                Id = testUser.Id,
                Roles =
                {
                    db.Roles.Find(testUser.Role.Id) ??
                    throw new InvalidOperationException($"Not found role '{testUser.Role}'"),
                },
            });

            addAny = true;
        }

        if (addAny)
        {
            return db.SaveChangesAsync();
        }

        return Task.CompletedTask;
    }

    public void AddMiddleware(WebApplication app)
    {
        app.Use(async (context, func) =>
        {
            const string TestingHeaderUserId = "X-Integration-Testing-User-Id";
            if (!context.Request.Headers.TryGetValue(TestingHeaderUserId, out var value))
            {
                await func();
                return;
            }

            if (!Guid.TryParse(value, out var id))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsJsonAsync(
                    new MessageResponse { Message = $"Can't parse test id from header '{value}'.", },
                    context.RequestAborted);
                return;
            }

            var user = _users.Where(e => e.Id == id)
                .Select(e => new User(e.Id, e.Nickname, e.TwitchIdentity) { Roles = { new Role(e.Role), }, })
                .FirstOrDefault();
            if (user is null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await context.Response.WriteAsJsonAsync(
                    new MessageResponse { Message = $"Not found user by id '{id}'.", },
                    context.RequestAborted);
                return;
            }

            // TODO
            context.User = new ClaimsPrincipal(new List<ClaimsIdentity>());
            context.User.EnrichRolesWithId(user);
            await func();
        });
    }

    private record TestUser(Guid Id, string Nickname, string TwitchIdentity, RoleName Role);
}
