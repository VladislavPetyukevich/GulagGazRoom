using Interview.Infrastructure.Chat.TokenProviders;
using Microsoft.EntityFrameworkCore;

namespace Interview.DependencyInjection;

public sealed class DependencyInjectionAppServiceOption
{
    public Action<DbContextOptionsBuilder> DbConfigurator { get; }
    public TwitchTokenProviderOption TwitchTokenProviderOption { get; }

    public DependencyInjectionAppServiceOption(TwitchTokenProviderOption twitchTokenProviderOption, Action<DbContextOptionsBuilder> dbConfigurator)
    {
        DbConfigurator = dbConfigurator;
        TwitchTokenProviderOption = twitchTokenProviderOption;
    }
}
