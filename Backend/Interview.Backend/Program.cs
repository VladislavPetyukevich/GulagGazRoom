using Interview.Backend;

using Interview.Infrastructure.Database;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("oauth.json", true);
builder.Configuration.AddEnvironmentVariables("INTERVIEW_BACKEND_");

// Add services to the container.
var serviceConfigurator = new ServiceConfigurator(builder.Environment, builder.Configuration);
serviceConfigurator.AddServices(builder.Services);

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto,
});

using (var serviceScope = app.Services.CreateScope())
{
    var appDbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();

    appDbContext.Database.Migrate();

    var testUserId = Guid.Parse("b5a05f34-e44d-11ed-b49f-e8e34e3377ec");
    if (app.Environment.IsDevelopment() && !appDbContext.Users.Any(e => e.Id == testUserId))
    {
        appDbContext.Users.Add(new User("TEST_BACKEND_DEV_USER", "d1731c50-e44d-11ed-905c-d08c09609150")
        {
            Id = testUserId,
            Avatar = null,
            Roles =
            {
                appDbContext.Roles.Find(RoleName.User.Id) !,
            },
        });
        appDbContext.SaveChanges();
    }
}

var middlewareConfigurator = new MiddlewareConfigurator(app);
middlewareConfigurator.AddMiddlewares();

app.Run();
