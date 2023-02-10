using Microsoft.EntityFrameworkCore;

namespace Interview.DependencyInjection;

public sealed class DependencyInjectionAppServiceOption
{
    public Action<DbContextOptionsBuilder> DbConfigurator { get; }

    public DependencyInjectionAppServiceOption(Action<DbContextOptionsBuilder> dbConfigurator)
    {
        DbConfigurator = dbConfigurator;
    }
}