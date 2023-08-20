using Interview.Domain.Events.ChangeEntityProcessors;
using Interview.Infrastructure.Database;
using Interview.Infrastructure.Database.Processors;
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

        var context = new AppDbContext(option.Options)
        {
            SystemClock = clock,
            Processors = new LazyPreProcessors(serviceCollection.BuildServiceProvider())
        };

        context.Database.EnsureCreated();
        return context;
    }
}
