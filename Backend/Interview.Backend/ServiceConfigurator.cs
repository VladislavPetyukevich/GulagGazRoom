using Ardalis.SmartEnum.SystemTextJson;
using Interview.Backend.Auth;
using Interview.Backend.WebSocket;
using Interview.Backend.WebSocket.UserByRoom;
using Interview.DependencyInjection;
using Interview.Infrastructure.Chat;
using Microsoft.EntityFrameworkCore;

namespace Interview.Backend;

public class ServiceConfigurator
{
    private readonly IHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public ServiceConfigurator(IHostEnvironment environment, IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;
    }

    public void AddServices(IServiceCollection serviceCollection)
    {
        if (_environment.IsDevelopment())
        {
            serviceCollection.AddCors(options =>
            {
                options.AddPolicy("All", policy =>
                {
                    policy
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                });
            });
        }

        serviceCollection
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new SmartEnumNameConverter<RoleName, int>());
            });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        serviceCollection.AddEndpointsApiExplorer();
        serviceCollection.AddSwaggerGen();

        var oAuthServiceDispatcher = new OAuth2Service(_configuration);

        var adminUsers = _configuration.GetSection(nameof(AdminUsers))
            .Get<AdminUsers>() ?? throw new ArgumentException($"Not found \"{nameof(AdminUsers)}\" section");

        var twitchService = oAuthServiceDispatcher.GetAuthService("twitch");

        var serviceOption = new DependencyInjectionAppServiceOption(
            new TwitchTokenProviderOption
            {
                ClientSecret = twitchService.ClientSecret,
                ClientId = twitchService.ClientId,
            },
            adminUsers,
            optionsBuilder =>
            {
                if (_environment.IsDevelopment())
                {
                    optionsBuilder.UseSqlite(_configuration.GetConnectionString("sqlite"));
                }
            });

        serviceCollection.AddAppServices(serviceOption);

        serviceCollection.AddAppAuth(twitchService);

        serviceCollection.AddHostedService<JobWriter>();
        serviceCollection.AddHostedService<EventSenderJob>();
        serviceCollection.AddSingleton<UserByRoomEventSubscriber>();
        serviceCollection.AddSingleton(oAuthServiceDispatcher);
    }
}
