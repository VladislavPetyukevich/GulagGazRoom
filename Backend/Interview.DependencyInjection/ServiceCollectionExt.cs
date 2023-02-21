using Interview.Domain.Certificates;
using Interview.Domain.Questions;
using Interview.Domain.Rooms;
using Interview.Domain.Users;
using Interview.Domain.Users.Roles;
using Interview.Infrastructure.Certificates.Pdf;
using Interview.Infrastructure.Chat.TokenProviders;
using Interview.Infrastructure.Database;
using Interview.Infrastructure.Questions;
using Interview.Infrastructure.Rooms;
using Interview.Infrastructure.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Interview.DependencyInjection;

public static class ServiceCollectionExt
{
    public static IServiceCollection AddAppServices(this IServiceCollection self, DependencyInjectionAppServiceOption option)
    {
        self.AddScoped<IUserRepository, UserRepository>();
        self.AddScoped<IRoomRepository, RoomRepository>();
        self.AddScoped<IQuestionRepository, QuestionRepository>();
        self.AddScoped<IRoleRepository, RoleRepository>();
        self.AddDbContext<AppDbContext>(option.DbConfigurator);
        self.AddSingleton<ICertificateGenerator, PdfCertificateGenerator>();
        
        self.AddScoped<UserService>();

        self.AddSingleton(option.TwitchTokenProviderOption);
        self.AddSingleton<ITwitchTokenProvider, TwitchTokenProvider>();
        self.Decorate<ITwitchTokenProvider, ReloadableCacheTwitchTokenProvider>();
        return self;
    }
}
