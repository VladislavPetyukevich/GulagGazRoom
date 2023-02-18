using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Interview.DependencyInjection;

public sealed class DependencyInjectionAppServiceOption
{
    public Action<DbContextOptionsBuilder> DbConfigurator { get; }
    public IConfiguration Configuration { get; }

    public DependencyInjectionAppServiceOption(IConfiguration configuration, Action<DbContextOptionsBuilder> dbConfigurator)
    {
        DbConfigurator = dbConfigurator;
        Configuration = configuration;
    }
}
