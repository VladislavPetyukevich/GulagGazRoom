using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Interview.Backend.Auth;
using Interview.Backend.RoomReactions;
using Interview.Backend.WebSocket;
using Interview.Backend.WebSocket.ConnectListener;
using Interview.Backend.WebSocket.UserByRoom;
using Interview.DependencyInjection;
using Interview.Domain;
using Interview.Domain.Connections;
using Interview.Infrastructure.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        serviceCollection.AddEndpointsApiExplorer();
        serviceCollection.AddSwaggerGen();

        var oAuthServiceDispatcher = new OAuthServiceDispatcher(_configuration);

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
                var connectionString = _configuration.GetConnectionString("database");
                if (_environment.IsDevelopment())
                {
                    optionsBuilder.UseSqlite(connectionString);
                }
                else if (_environment.IsPreProduction())
                {
                    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                    optionsBuilder.UseNpgsql(connectionString);
                }
                else
                {
                    throw new InvalidOperationException("Unknown environment");
                }
            });

        serviceCollection.AddAppServices(serviceOption);

        serviceCollection.AddAppAuth(twitchService);

        serviceCollection.AddHostedService<EventSenderJob>();
        serviceCollection.AddHostedService<WebSocketConnectListenJob>();

        serviceCollection.AddSingleton<UserByRoomEventSubscriber>();
        serviceCollection.AddSingleton(oAuthServiceDispatcher);
        serviceCollection.AddSingleton<UserClaimService>();

        serviceCollection.Configure<ChatBotAccount>(_configuration.GetSection(nameof(ChatBotAccount)));
    }
}
