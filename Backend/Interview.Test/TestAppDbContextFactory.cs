using Interview.Domain.Events.ChangeEntityProcessors;
using Interview.Domain.Users;
using Interview.Infrastructure.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;

namespace Interview.Test;

public class TestAppDbContextFactory
{
    public AppDbContext Create(ISystemClock clock)
    {
        var sqliteConnection = new SqliteConnection("Data Source=:memory:");
        sqliteConnection.Open();

        var option = new DbContextOptionsBuilder().UseSqlite(
            sqliteConnection
        );

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();

        var context = new AppDbContext(option.Options, Array.Empty<IChangeEntityProcessor>())
        {
            SystemClock = clock,
            LazyCurrentUserAccessor = new LazyCurrentUserAccessor(serviceCollection.BuildServiceProvider())
        };
        context.Database.EnsureCreated();
        return context;
    }
}
