using System.Reflection;
using Interview.Backend;

using Interview.Infrastructure.Database;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables("INTERVIEW_BACKEND_");

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Gulag Open API",
        Version = "v1",
        Description = "Gulag Service Interface",
        Contact = new OpenApiContact
        {
            Name = "Vladislav Petyukevich",
            Url = new Uri("https://github.com/VladislavPetyukevich"),
            Email = "gulaglinkfun@yandex.ru",
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license"),
        },
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

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
