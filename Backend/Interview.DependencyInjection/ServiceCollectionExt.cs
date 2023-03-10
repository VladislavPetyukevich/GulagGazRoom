using Interview.Domain.Certificates;
using Interview.Domain.Events;
using Interview.Domain.Questions;
using Interview.Domain.Rooms;
using Interview.Domain.Rooms.Service;
using Interview.Domain.Users;
using Interview.Domain.Users.Roles;
using Interview.Infrastructure.Certificates.Pdf;
using Interview.Infrastructure.Chat.TokenProviders;
using Interview.Infrastructure.Database;
using Interview.Infrastructure.Questions;
using Interview.Infrastructure.Rooms;
using Interview.Infrastructure.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;

namespace Interview.DependencyInjection;

public static class ServiceCollectionExt
{
    public static IServiceCollection AddAppServices(this IServiceCollection self, DependencyInjectionAppServiceOption option)
    {
        self.AddDbContextPool<AppDbContext>(option.DbConfigurator);

        self.AddScoped<IUserRepository, UserRepository>();
        self.AddScoped<IRoomRepository, RoomRepository>();
        self.AddScoped<IQuestionRepository, QuestionRepository>();
        self.AddScoped<IRoleRepository, RoleRepository>();
        self.AddSingleton<ICertificateGenerator, PdfCertificateGenerator>();
        self.AddSingleton<IRoomEventDispatcher, RoomEventDispatcher>();
        self.AddSingleton<ISystemClock, SystemClock>();
        self.AddSingleton(option.AdminUsers);

        // Services
        self.AddScoped<UserService>();
        self.AddScoped<RoomService>();
        self.AddScoped<QuestionService>();

        self.AddSingleton(option.TwitchTokenProviderOption);
        self.AddSingleton<ITwitchTokenProvider, TwitchTokenProvider>();
        self.Decorate<ITwitchTokenProvider, ReloadableCacheTwitchTokenProvider>();
        return self;
    }
}
