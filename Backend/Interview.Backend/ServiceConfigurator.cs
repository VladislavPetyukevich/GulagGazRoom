using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Ardalis.SmartEnum.SystemTextJson;
using Interview.Backend.Auth;
using Interview.Backend.Swagger;
using Interview.Backend.WebSocket;
using Interview.Backend.WebSocket.ConnectListener;
using Interview.Backend.WebSocket.UserByRoom;
using Interview.DependencyInjection;
using Interview.Domain.RoomQuestions;
using Interview.Infrastructure.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

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
                options.JsonSerializerOptions.Converters.Add(new SmartEnumNameConverter<RoomQuestionState, int>());
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
                    optionsBuilder.UseSqlite(
                        connectionString,
                        builder => builder.MigrationsAssembly(typeof(Migrations.Sqlite.AppDbContextFactory).Assembly.FullName));
                }
                else if (_environment.IsPreProduction())
                {
                    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                    optionsBuilder.UseNpgsql(
                        connectionString,
                        builder => builder.MigrationsAssembly(typeof(Migrations.Postgres.AppDbContextFactory).Assembly.FullName));
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

        serviceCollection.AddRateLimiter(_ =>
        {
            _.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, IPAddress>(context =>
            {
                var address = context?.Connection?.RemoteIpAddress;
                if (address is not null && !IPAddress.IsLoopback(address))
                {
                    return RateLimitPartition.GetFixedWindowLimiter(address, key => new()
                    {
                        PermitLimit = 36,
                        Window = TimeSpan.FromSeconds(30),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        AutoReplenishment = true,
                    });
                }

                return RateLimitPartition.GetNoLimiter(IPAddress.Loopback);
            });
            _.OnRejected = (context, token) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                }

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.WriteAsync(
                    "Too many requests. Please try again later.",
                    cancellationToken: token);

                return ValueTask.CompletedTask;
            };
        });

        serviceCollection.AddSwaggerGen(options =>
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
            options.CustomSchemaIds(type => type.ToString());

            var swaggerOption = _configuration.GetSection(nameof(SwaggerOption)).Get<SwaggerOption>() ??
                         throw new InvalidOperationException(nameof(SwaggerOption));
            if (!string.IsNullOrEmpty(swaggerOption.RoutePrefix))
            {
                options.DocumentFilter<SwaggerDocumentFilter>(swaggerOption.RoutePrefix);
            }
        });
    }
}
