using Ardalis.SmartEnum.SystemTextJson;
using Interview.Backend.Auth;
using Interview.Backend.WebSocket;
using Interview.Backend.WebSocket.UserByRoom;
using Interview.DependencyInjection;
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
        serviceCollection
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new SmartEnumNameConverter<RoleName, int>());
            });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        serviceCollection.AddEndpointsApiExplorer();
        serviceCollection.AddSwaggerGen();

        var oAuthTwitchOptions = new OAuthTwitchOptions(_configuration);
        var serviceOption = new DependencyInjectionAppServiceOption(oAuthTwitchOptions.ToTwitchTokenProviderOption(), optionsBuilder =>
        {
            if (_environment.IsDevelopment())
            {
                optionsBuilder.UseSqlite(_configuration.GetConnectionString("sqlite"));
            }
        });
        serviceCollection.AddAppServices(serviceOption);
        serviceCollection.AddAppAuth(oAuthTwitchOptions);

        serviceCollection.AddHostedService<JobWriter>();
        serviceCollection.AddHostedService<EventSenderJob>();
        serviceCollection.AddSingleton<UserByRoomEventSubscriber>();
    }
}
