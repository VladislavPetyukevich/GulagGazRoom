using Interview.Domain.Users;
using Interview.Infrastructure.Chat;
using Microsoft.EntityFrameworkCore;

namespace Interview.DependencyInjection;

public sealed class DependencyInjectionAppServiceOption
{
    public Action<DbContextOptionsBuilder> DbConfigurator { get; }
    public TwitchTokenProviderOption TwitchTokenProviderOption { get; }

    public AdminUsers AdminUsers { get; }

    public DependencyInjectionAppServiceOption(TwitchTokenProviderOption twitchTokenProviderOption, AdminUsers adminUsers, Action<DbContextOptionsBuilder> dbConfigurator)
    {
        DbConfigurator = dbConfigurator;
        AdminUsers = adminUsers;
        TwitchTokenProviderOption = twitchTokenProviderOption;
    }
}
