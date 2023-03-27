using Interview.Backend;

using Interview.Infrastructure.Database;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);
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
    appDbContext.Database.EnsureCreated();
}

var middlewareConfigurator = new MiddlewareConfigurator(app);
middlewareConfigurator.AddMiddlewares();

app.Run();
